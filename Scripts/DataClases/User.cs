using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
internal struct User {
    static readonly HMACSHA512 hmacsha512 = new(new byte[]{2, 4, 5, 26});
    static internal byte[] peper = new byte[] {4, 7, 234, 78};
    public byte[] Username; 
    private byte[] EncPassword;
    private byte[] Salt;
    public byte[] userDataEncrypted {
        get {
            // Salik visu kopā
            byte[] bytes = new byte[88];
            Buffer.BlockCopy(Username, 0, bytes, 0, 20);
            Buffer.BlockCopy(EncPassword, 0, bytes, 20, 64);
            Buffer.BlockCopy(Salt, 0, bytes, 84, 4);
            return bytes;
        }
        set {
            // Iedod vērtības
            Username =  new byte[20];
            EncPassword = new byte[64];
            Salt = new byte[4];

            Buffer.BlockCopy(value, 0, Username, 0, 20);
            Buffer.BlockCopy(value, 20, EncPassword, 0, 64);
            Buffer.BlockCopy(value, 84, Salt, 0, 4);
        }}
    // Fiksētā laikā pārbauda vai paroles ir vienādas
    // Izslēdzu "Ilining" un optimizācijas
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public readonly bool PasswordEqual(byte[] bytesPasswordPepered) {
        
        // Pielieku klāt "Sāli"
        byte[] encPassword = new byte[72];

        Buffer.BlockCopy(bytesPasswordPepered, 0, encPassword, 0, 72);
        Buffer.BlockCopy(Salt, 0, encPassword, 64, 4);

        // Pārbaudu paroles konstantā laikā
        return CryptographicOperations.FixedTimeEquals(EncPassword, hmacsha512.ComputeHash(encPassword));
    }
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public User(string username, string userPassword)
    {
        // Iegūst kriptogrāfiski drošus nejaušus baitus sāļošanai (Nevar redzēt ka paroli izmanto vairāki cilvēki)
        byte[] bytes = new byte[4];
        while (true) {
            try {
                using (var rng = new RNGCryptoServiceProvider()) {
                    rng.GetBytes(bytes);
                }
                break;
            } catch (CryptographicException) {};
        }
        
        Salt = bytes;

        // Saglabā lietotājvādru objektā
        byte[] bytes1Str = new byte[20];
        byte[] usernameBytes = Encoding.UTF8.GetBytes(username);
        Buffer.BlockCopy(usernameBytes, 0, bytes1Str, 0, usernameBytes.Length);
        Username = bytes1Str;

        // Saglabā paroles hash objektā
        byte[] bytesPassword = Encoding.UTF8.GetBytes(userPassword);
        byte[] hashPassword = hmacsha512.ComputeHash(bytesPassword);
        byte[] encPassword = new byte[72];

        Buffer.BlockCopy(hashPassword, 0, encPassword, 0, 64);
        Buffer.BlockCopy(Salt, 0, encPassword, 64, 4);
        Buffer.BlockCopy(peper, 0, encPassword, 68, 4);

        EncPassword = hmacsha512.ComputeHash(encPassword);
    }
}