using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FinalesFunkeln.Util;

namespace FinalesFunkeln.Util
{
    class ProcessInjector:IDisposable
    {
        readonly byte[] _connectCc = 
		{
			0x55,										//PUSH EBP
			0x8B, 0xEC,									//MOV EBP, ESP
			0x60, 										//PUSHAD
			0x8B, 0x45, 0x0C, 							//MOV EAX, [EBP+C]
			0x66, 0x83, 0x38, 0x02, 					//CMP WORD PTR [EAX], 2
			0x75, 0x12, 								//JNZ SHORT 12h
			0xB9, 0x08, 0x33, 0x00, 0x00,				//MOV ECX, 3308
			0x66, 0x39, 0x48, 0x02,						//CMP [EAX+2], CX
			0x75, 0x07,									//JNZ SHORT 7h
			0xC7, 0x40, 0x04, 0x7F, 0x00, 0x00, 0x01,	//MOV DWORD PTR [EAX+4], 100007Fh
			0x61,										//POPAD
			0xE9, 0x00, 0x00, 0x00, 0x00				//JMP LONG <X>	
		};

        readonly byte[] _safeCheck =
		{
			0x8B, 0xFF,									//MOVE EDI, EDI
			0x55,										//PUSH EBP
			0x8B, 0xEC									//MOV EBP, ESP	
		};

        public string ProcessName { get; protected set; }
        public Thread CheckThread { get; protected set; }
        public Process CurrentProcess { get; protected set; }

        public event EventHandler Injected;
        public event EventHandler<Process> ProcessFound;
        public event EventHandler<Process> ProcessExited;

		bool _isInjected;

        EventWaitHandle _waitHandle;
        public bool IsInjected
        {
            get { return _isInjected; }
            protected set
            {
                if (_isInjected != value)
                {
                    _isInjected = value;
                    if (Injected != null)
                        Injected(this, new EventArgs());
                }
            }
        }

        public ProcessInjector(string process)
        {
            ProcessName = process;
            CheckThread = new Thread(CheckLoop) { IsBackground = true };
            _waitHandle = new AutoResetEvent(false);
        }

        public void Start()
        {
            if (!CheckThread.IsAlive)
                CheckThread.Start();
        }
        public void Clear()
        {
            CurrentProcess = null;
        }

        void CheckLoop()
        {
            while (CheckThread != null)
            {
                IsInjected = false;
                var processes = Process.GetProcessesByName(ProcessName);
                CurrentProcess = processes.FirstOrDefault();
                if (CurrentProcess != null)
                {
                    if (ProcessFound != null)
                        ProcessFound(this, CurrentProcess);

                    CurrentProcess.WaitForExit();

                    if (ProcessExited != null)
                        ProcessExited(this, CurrentProcess);
                }
                else
                    Task.Delay(100).Wait();
            }
        }

        internal void Inject()
        {
            Thread.Sleep(1000);
            while (true)
            {
                using (var mem = new ProcessMemory(CurrentProcess.Id))
                {
                    using (var notemem = new ProcessMemory(Process.GetCurrentProcess().Id))
                    {
                        if (mem.Is64Bit())
                            throw new NotSupportedException("lolclient is running in 64bit mode which is not supported");

                        var connect = new byte[_connectCc.Length];
                        _connectCc.CopyTo(connect, 0);
                        int jmpaddrloc = connect.Length - 4;

                        var mod = ProcessMemory.GetModule("ws2_32.dll");
                        Int32 reladdr = notemem.GetAddress(mod, "connect");
                        reladdr -= mod;

                        var lolmod = GetModuleAddress(CurrentProcess, mem, "ws2_32.dll");
                        if (lolmod == 0)
                        {
                            //throw new FileNotFoundException("Lolclient has not yet loaded ws2_32.dll");
                            Thread.Sleep(1000);
                            continue;
                        }
                        Int32 connectaddr = lolmod + reladdr;

                        var bytes = mem.Read(connectaddr, 5);
                        if (bytes[0] == 0xe9)
                        {
                            IsInjected = true;
                            throw new WarningException("Connect already redirected");
                        }
                        if (!bytes.SequenceEqual(_safeCheck))
                        {
                            bytes = mem.Read(connectaddr, 20);
                            throw new AccessViolationException(string.Format("Connect has unknown bytes [{0}]", Convert.ToBase64String(bytes)));
                        }

                        Int32 addr = mem.Alloc(_connectCc.Length);
                        BitConverter.GetBytes((connectaddr + 5) - (addr + connect.Length)).CopyTo(connect, jmpaddrloc);
                        mem.Write(addr, connect);

                        var jmp = new byte[5];
                        jmp[0] = 0xE9;
                        BitConverter.GetBytes(addr - (connectaddr + 5)).CopyTo(jmp, 1);
                        mem.Write(connectaddr, jmp);
                    }
                }
                IsInjected = true;
                break;
            }
        }

        Int32 GetModuleAddress(Process curproc, ProcessMemory curmem, string name)
        {
                var mods = curmem.GetModuleInfos();
                var mod = mods.FirstOrDefault(mi => mi.baseName.ToLowerInvariant() == name);
                if (mod == null)
                    return 0;
                return mod.baseOfDll.ToInt32();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~ProcessInjector()
        {
            Dispose(false);
        }
        void Dispose(bool dispose)
        {
            if (dispose)
            {
                CheckThread = null;
            }
        }


    }
}
