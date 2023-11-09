import Keycloak from "keycloak-js";

const keycloakInstance = new Keycloak('/keycloak.json');

/**
 * Initializes Keycloak instance and calls the provided callback function if successfully authenticated.
 *
 * @param onAuthenticatedCallback
 */
const Login = (onAuthenticatedCallback: Function) => {
  keycloakInstance
    .init({ onLoad: "login-required" })
    .then(function (authenticated) {
      authenticated ? onAuthenticatedCallback() : alert("non authenticated");
    })
    .catch((e) => {
      console.dir(e);
      console.log(`keycloak init exception: ${e}`);
    });
};

const UserName = () => keycloakInstance.tokenParsed?.preferred_username;
const UserGroups = () => keycloakInstance.tokenParsed?.groups;

const UserClientRoles = () => {
  if (keycloakInstance.resourceAccess === undefined) return undefined;
  else return keycloakInstance.resourceAccess["MyApp"].roles;
};

const UserRealmRoles = () => {
  if (keycloakInstance.realmAccess === undefined) return undefined;
  else return keycloakInstance.realmAccess?.roles;
}

const Logout = keycloakInstance.logout;

const isLoggedIn = () => !!keycloakInstance.token;

const getToken = () => keycloakInstance.token;

const doLogin = keycloakInstance.login;

const updateToken = (successCallback: any) =>
  keycloakInstance.updateToken(5).then(successCallback).catch(doLogin);

const KeyCloakService = {
  CallLogin: Login,
  GetUserName: UserName,
  GetUserGroups: UserGroups,
  GetUserClientRoles: UserClientRoles,
  GetUserRealmRoles: UserRealmRoles,
  CallLogout: Logout,
  IsLoggedIn: isLoggedIn,
  GetToken: getToken,
  UpdateToken: updateToken,
};

export default KeyCloakService;