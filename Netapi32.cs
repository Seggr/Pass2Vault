using System.Runtime.InteropServices;
using System.ComponentModel;

public class Netadpi32
{
    [DllImport("Netapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int NetUserChangePassword(
        string domainname,
        string username,
        string oldpassword,
        string newpassword
        );
    public static void ChangePassword(string username, string domain, string oldpassword, string newpassword)
    {
        int result = NetUserChangePassword(
            domain,
            username,
            oldpassword,
            newpassword
            );
        if (result != 0)
            throw new Win32Exception();
    }
}