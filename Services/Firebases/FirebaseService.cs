using System;
using System.Text.Json;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using OptimizingLastMile.Configure;
using OptimizingLastMile.Models.Commons;

namespace OptimizingLastMile.Services.Firebases;

public class FirebaseService : IFirebaseService
{
    private readonly ServiceAccountFirebaseConfig _firebaseConfig;

    public FirebaseService(IOptionsMonitor<ServiceAccountFirebaseConfig> options)
    {
        _firebaseConfig = options.CurrentValue;

        string jsonServiceAccount = JsonSerializer.Serialize(_firebaseConfig);

        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromJson(jsonServiceAccount)
        });
    }

    public async Task<GenericResult<string>> VerifyIdToken(string idToken)
    {
        string uid = null;

        try
        {
            // verify idtoken, receive firebase token if verified
            var token = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);

            if (token != null)
            {
                uid = token.Uid;
            }
        }
        catch (Exception)
        {
            var error = Errors.Firebase.InvalidTokenFirebase();
            return GenericResult<string>.Fail(error);
        }

        return GenericResult<string>.Ok(uid);
    }

    public async Task<GenericResult<UserRecord>> GetUserInfo(string uid)
    {
        UserRecord userRecord;
        try
        {
            // get userinfo by uid
            userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        }
        catch (Exception)
        {
            var error = Errors.Firebase.InvalidTokenFirebase();
            return GenericResult<UserRecord>.Fail(error);
        }

        return GenericResult<UserRecord>.Ok(userRecord);
    }
}

