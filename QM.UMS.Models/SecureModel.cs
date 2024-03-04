// ------------------------------------------------------------------------------------------------------------
// <copyright file="SecureClientModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Models
{
    #region Namespaces
    using CommonApplicationFramework.Security;
    #endregion

    /// -----------------------------------------------------------------
    /// Namespace:      <ServtrackerModels>
    /// Class:          <SecureClientModel>
    /// Description:    <Description>
    /// Author:         <Nirav>                    
    /// -----------------------------------------------------------------
    public class SecureModel
    {
        private readonly PasswordHasher _PasswordHasher = new PasswordHasher();             
        public string Salt { get; set; }
        public string Password { get; set; }

        private readonly PasswordHasher _PasswordHasherCustomer = new PasswordHasher();
        public string SaltCustomer { get; set; }
        public string PasswordCustomer { get; set; }

        public SecureModel(UserProfileModel  client)
        {            
            this.SetSaltAndPassword(client.Password);
        }
      
        private void SetSaltAndPassword(string password)
        {
            string salt; string hashedPassword;
            this._PasswordHasher.HashPassword(password, out hashedPassword, out salt);
            this.Salt = salt;
            this.Password = hashedPassword;
        }

        public SecureModel(Customer client)
        {
            this.SetSaltAndPasswordCustomer(client.Password);
        }

        private void SetSaltAndPasswordCustomer(string passwordCustomer)
        {
            string saltCustomer;
            string hashedPasswordCustomer;
            this._PasswordHasherCustomer.HashPassword(passwordCustomer,out hashedPasswordCustomer,out saltCustomer);
            this.SaltCustomer = saltCustomer;
            this.PasswordCustomer = hashedPasswordCustomer;
        }  
    }
}
