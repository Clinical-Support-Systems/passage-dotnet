using System;
using System.Collections.Generic;
using System.Text;

namespace PassageIdentity
{
    public class PassageAuthentication
    {
        public PassageApp GetApp(string appId)
        {
            throw new NotImplementedException();
        }

        public PassageUser GetUser(string identifier)
        {
            throw new NotImplementedException();
        }


        public void StartWebAuthnLogin(string appId)
        {
            // https://auth.passage.id/v1/apps/{app_id}/login/webauthn/start/
            // https://auth.passage.id/v1/apps/{app_id}/login/webauthn/finish/
            throw new NotImplementedException();
        }
    }
}
