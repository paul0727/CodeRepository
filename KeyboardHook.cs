using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using System.Windows.Input;

namespace WpfApplication1
{
    /// <summary>
    /// ���̹���
    /// [���´�������ĳ���ѣ����Ǳ���ԭ��]
    /// </summary>
    class KeyboardHook
    {
        public event System.Windows.Forms.KeyEventHandler KeyDownEvent;
        public event KeyPressEventHandler KeyPressEvent;
        public event System.Windows.Forms.KeyEventHandler KeyUpEvent;

        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        static int hKeyboardHook = 0; //�������̹��Ӵ���ĳ�ʼֵ
        //ֵ��Microsoft SDK��Winuser.h���ѯ
        // http://www.bianceng.cn/Programming/csharp/201410/45484.htm
        public const int WH_KEYBOARD_LL = 13;   //�̼߳��̹��Ӽ��������Ϣ��Ϊ2��ȫ�ּ��̼��������Ϣ��Ϊ13
        HookProc KeyboardHookProcedure; //����KeyboardHookProcedure��ΪHookProc����
        //���̽ṹ
        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct
        {
            public int vkCode;  //��һ��������롣�ô��������һ����ֵ�ķ�Χ1��254
            public int scanCode; // ָ����Ӳ��ɨ����Ĺؼ�
            public int flags;  // ����־
            public int time; // ָ����ʱ����ǵ����ѶϢ
            public int dwExtraInfo; // ָ��������Ϣ��ص���Ϣ
        }
        //ʹ�ô˹��ܣ���װ��һ������
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);


        //���ô˺���ж�ع���
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);


        //ʹ�ô˹��ܣ�ͨ����Ϣ���Ӽ�����һ������
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        // ȡ�õ�ǰ�̱߳�ţ��̹߳�����Ҫ�õ���
        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();

        //ʹ��WINDOWS API���������ȡ��ǰʵ���ĺ���,��ֹ����ʧЧ
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        public void Start()
        {
            // ��װ���̹���
            if (hKeyboardHook == 0)
            {
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName), 0);
                //hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                //************************************
                //�����̹߳���
                //SetWindowsHookEx( 2,KeyboardHookProcedure, IntPtr.Zero, GetCurrentThreadId());//ָ��Ҫ�������߳�idGetCurrentThreadId(),
                //����ȫ�ֹ���,��Ҫ���ÿռ�(using System.Reflection;)
                //SetWindowsHookEx( 13,MouseHookProcedure,Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),0);
                //
                //����SetWindowsHookEx (int idHook, HookProc lpfn, IntPtr hInstance, int threadId)���������Ӽ��뵽���������У�˵��һ���ĸ�������
                //idHook �������ͣ���ȷ�����Ӽ���������Ϣ������Ĵ�������Ϊ2��������������Ϣ�������̹߳��ӣ������ȫ�ֹ��Ӽ���������ϢӦ��Ϊ13��
                //�̹߳��Ӽ��������Ϣ��Ϊ7��ȫ�ֹ��Ӽ��������Ϣ��Ϊ14��lpfn �����ӳ̵ĵ�ַָ�롣���dwThreadId����Ϊ0 ����һ���ɱ�Ľ��̴�����
                //�̵߳ı�ʶ��lpfn����ָ��DLL�еĹ����ӳ̡� �������⣬lpfn����ָ��ǰ���̵�һ�ι����ӳ̴��롣���Ӻ�������ڵ�ַ�������ӹ����κ�
                //��Ϣ���������������hInstanceӦ�ó���ʵ���ľ������ʶ����lpfn��ָ���ӳ̵�DLL�����threadId ��ʶ��ǰ���̴�����һ���̣߳�������
                //�̴���λ�ڵ�ǰ���̣�hInstance����ΪNULL�����Ժܼ򵥵��趨��Ϊ��Ӧ�ó����ʵ�������threaded �밲װ�Ĺ����ӳ���������̵߳ı�ʶ��
                //���Ϊ0�������ӳ������е��̹߳�������Ϊȫ�ֹ���
                //************************************
                //���SetWindowsHookExʧ��
                if (hKeyboardHook == 0)
                {
                    Stop();
                    throw new Exception("��װ���̹���ʧ��");
                }
            }
        }
        public void Stop()
        {
            bool retKeyboard = true;


            if (hKeyboardHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }

            if (!(retKeyboard)) throw new Exception("ж�ع���ʧ�ܣ�");
        }
        //ToAsciiְ�ܵ�ת��ָ�����������ͼ���״̬����Ӧ�ַ����ַ�
        [DllImport("user32")]
        public static extern int ToAscii(int uVirtKey, //[in] ָ������ؼ�������з��롣
                                         int uScanCode, // [in] ָ����Ӳ��ɨ����Ĺؼ��뷭���Ӣ�ġ��߽�λ�����ֵ�趨�Ĺؼ�������ǣ���ѹ��
                                         byte[] lpbKeyState, // [in] ָ�룬��256�ֽ����飬������ǰ���̵�״̬��ÿ��Ԫ�أ��ֽڣ����������״̬��һ���ؼ�������߽�λ���ֽ���һ�ף��ؼ����µ������£����ڵͱ��أ�������ñ������ؼ��Ƕ��л����ڴ˹��ܣ�ֻ����λ��CAPS LOCK������صġ����л�״̬��NUM�����͹��������������ԡ�
                                         byte[] lpwTransKey, // [out] ָ��Ļ������յ������ַ����ַ���
                                         int fuState); // [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise.

        //��ȡ������״̬
        [DllImport("user32")]
        public static extern int GetKeyboardState(byte[] pbKeyState);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        private const int WM_KEYDOWN = 0x100;//KEYDOWN
        private const int WM_KEYUP = 0x101;//KEYUP
        private const int WM_SYSKEYDOWN = 0x104;//SYSKEYDOWN
        private const int WM_SYSKEYUP = 0x105;//SYSKEYUP

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            // ���������¼�
            if ((nCode >= 0) && (KeyDownEvent != null || KeyUpEvent != null || KeyPressEvent != null))
            {
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                // raise KeyDown
                if (KeyDownEvent != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    System.Windows.Forms.KeyEventArgs e = new System.Windows.Forms.KeyEventArgs(keyData);
                    KeyDownEvent(this, e);
                }

                //���̰���
                if (KeyPressEvent != null && wParam == WM_KEYDOWN)
                {
                    byte[] keyState = new byte[256];
                    GetKeyboardState(keyState);

                    byte[] inBuffer = new byte[2];
                    if (ToAscii(MyKeyboardHookStruct.vkCode, MyKeyboardHookStruct.scanCode, keyState, inBuffer, MyKeyboardHookStruct.flags) == 1)
                    {
                        KeyPressEventArgs e = new KeyPressEventArgs((char)inBuffer[0]);
                        KeyPressEvent(this, e);
                    }
                }

                // ����̧��
                if (KeyUpEvent != null && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    System.Windows.Forms.KeyEventArgs e = new System.Windows.Forms.KeyEventArgs(keyData);
                    KeyUpEvent(this, e);
                }

            }
            //�������1���������Ϣ�������Ϣ����Ϊֹ�����ٴ��ݡ�
            //�������0�����CallNextHookEx��������Ϣ����������Ӽ������´��ݣ�Ҳ���Ǵ�����Ϣ�����Ľ�����
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }
        ~KeyboardHook()
        {
            Stop();
        }
    }
}