import { createStore } from 'vuex'

export default createStore({
    state () {
        return {
            isAuth: undefined,
            access_token:undefined,
            userName:undefined,
        }
    },
    mutations: {
        authentificate (state, payload) {
            state.isAuth = payload.isAuth;
            state.access_token = payload.access_token;
            state.userName = payload.userName;
        }
    },
    getters:{
        auth(state){
            return state.isAuth;
        }
    }
})