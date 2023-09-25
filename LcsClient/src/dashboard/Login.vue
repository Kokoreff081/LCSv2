<template>
    <body class="hold-transition login-page">
    <div class="login-box">
        <div class="login-logo">
            <a href="/"><b>Light Control Service</b></a>
        </div>
        <!-- /.login-logo -->
        <div class="card">
            <div class="card-body login-card-body">
                <p class="login-box-msg">Sign in to start your session</p>

                <form asp-anti-forgery="true">
                    <div class="input-group mb-3">
                        <input type="text"  class="form-control" v-model="login" placeholder="Login"/>
                    </div>
                    <div class="input-group mb-3">
                        <input class="form-control" v-model="password" placeholder="Password"/>
                    </div>
                    <div class="row">
                        <div class="col-8">
                            <div class="icheck-primary">
                                <input type="checkbox" id="remember">
                                <label for="remember">
                                    Remember Me
                                </label>
                            </div>
                        </div>
                        <!-- /.col -->
                        <div class="col-4">
                            <button type="submit" class="btn btn-primary btn-block" @click="auth">Sign In</button>
                        </div>
                        <!-- /.col -->
                    </div>
                </form>
            </div>
        </div>
    </div>
    </body>
</template>

<script>
import { HTTP } from '../global/commonHttpRequest';
export default {
    name: "Login",
    componentns: {},
    data(){
        return {
            login:undefined,
            password:undefined
        }
    },
    methods: {
        auth(){
            let headers = { 'Content-Type': 'application/json' };
            let LoginModel = {Login : this.login, Password:this.password};
            HTTP.post('Account/Login', LoginModel, headers)
                .then(response => {
                    let arr = response.data;
                    if(arr.access_token!=''){
                        let payload = {isAuth:true, access_token:arr.access_token, userName : arr.username};
                        this.$store.commit('authentificate', payload);
                    };
                    this.$router.push('/');
                })
                .catch(e => {
                    console.log(e);
                })
        }
    }
}
</script>

<style scoped>

</style>