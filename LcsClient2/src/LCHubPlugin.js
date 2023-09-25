import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { createApp } from "vue";

export default {
    install (app) {
        // use new Vue instance as an event bus
        const lcHub = createApp();
        // every component will use this.$questionHub to access the event bus
        app.config.globalProperties.$lcHub = lcHub;

        let connection = null

        connection = new HubConnectionBuilder()
            .withUrl("/api/lchub")
            .withAutomaticReconnect()
            .build();

        connection.on('NewFrame', (frame) => {
            lcHub.$emit('new-frame', { frame });
        })

        function start() {
            connection.start();
        }

        start();
        // Forward server side SignalR events through $questionHub, where components will listen to them
        
    }
};