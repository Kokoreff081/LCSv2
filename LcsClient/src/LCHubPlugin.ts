import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { createApp} from "vue";
import App from './app/app.vue';

export default {
    install (app:any) {
        // use new Vue instance as an event bus
        const lcHub:any = createApp(App);
        // every component will use this.$questionHub to access the event bus
        app.config.globalProperties.$lcHub = lcHub;

        let connection:any = null

        connection = new HubConnectionBuilder()
            .withUrl("/api/lchub")
            .withAutomaticReconnect()
            .build();

        connection.on('NewFrame', (frame:any) => {
            lcHub.$emit('new-frame', { frame });
        })

        function start() {
            connection.start();
        }

        start();
        // Forward server side SignalR events through $questionHub, where components will listen to them

    }
};