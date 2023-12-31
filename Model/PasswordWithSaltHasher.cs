﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


namespace Model
{
    public class PasswordWithSaltHasher
    {
        public HashedPasswordWithSalt HashWithSalt(string password, int saltLength, HashAlgorithm hashAlgo, byte[] saltBytes)
        {
            byte[] passwordAsBytes = Encoding.UTF8.GetBytes(password);
            List<byte> passwordWithSaltBytes = new List<byte>();
            passwordWithSaltBytes.AddRange(passwordAsBytes);
            passwordWithSaltBytes.AddRange(saltBytes);
            byte[] digestBytes = hashAlgo.ComputeHash(passwordWithSaltBytes.ToArray());
            return new HashedPasswordWithSalt(Convert.ToBase64String(saltBytes), Convert.ToBase64String(digestBytes));

        }
    }
}
