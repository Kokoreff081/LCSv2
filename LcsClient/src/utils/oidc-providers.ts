/* eslint-disable no-async-promise-executor */
import {UserManager, UserManagerSettings} from 'oidc-client-ts';
import {sleep} from './helpers';
import instance from './axios';

const GOOGLE_CONFIG: UserManagerSettings = {
    authority: 'https://accounts.google.com',
    client_id:
        '533830427279-cspigijdu0g50c7imca5pvdbrcn2buaq.apps.googleusercontent.com',
    client_secret: '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',
    redirect_uri: `${window.location.protocol}//${window.location.host}/callback`,
    scope: 'openid email profile',
    loadUserInfo: true
};

export const GoogleProvider = new UserManager(GOOGLE_CONFIG);

export const authLogin = (login: string, password: string) => {
    return new Promise(async (res, rej) => {
        let headers = { 'Content-Type': 'application/json' };
        let LoginModel = {Login:login, Password:password};
        instance.post('/Account/Login', LoginModel, {headers})
            .then(response=>{
                let data = response.data;
                if(data.access_tocken != "") {
                    let token:string = data.access_token;
                    let userProfile = {
                        login: data.username,
                        access_token: token,
                        role: data.role
                    }
                    console.log(userProfile);
                    localStorage.setItem('authentication', JSON.stringify({profile: userProfile}));
                    return res({ profile: userProfile});
                }
                else{
                    return rej({message: 'Credentials are wrong!'});
                }
            })
            .catch(e => {
                console.log(e);
            })
       
    });
};

export const getAuthStatus = () => {
    return new Promise(async (res) => {
        await sleep(500);
        try {
            let authentication = localStorage.getItem('authentication');
            if (authentication) {
                authentication = JSON.parse(authentication);
                return res(authentication);
            }
            return res(undefined);
        } catch (error) {
            return res(undefined);
        }
    });
};
