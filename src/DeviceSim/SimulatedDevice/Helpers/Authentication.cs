// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Threading.Tasks;
//using Foundation;
using SimulatedDevice.Utils;
using Microsoft.WindowsAzure.MobileServices;
using SimulatedDevice.Utils.Interfaces;
using System.Threading;
namespace SimulatedDevice.Helpers
{
    public class Authentication : IAuthentication
    {
        public async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            MobileServiceUser user = null;

            try
            {
                Settings.Current.LoginAttempts++;
                //user = client.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount, //add token reference here)
                Settings.Current.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                Settings.Current.AzureMobileUserId = user?.UserId ?? string.Empty;

                
            }
            catch (Exception e)
            {
                //Deliberately Ignore errors
            }

            return user;
        }

        public void ClearCookies()
        {
           // throw new NotImplementedException();
        }
    }
}