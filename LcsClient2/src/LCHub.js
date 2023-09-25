import { HubConnectionBuilder } from "@microsoft/signalr";

class LCHub {
    constructor() {
        this.client = new HubConnectionBuilder()
            .withUrl("/api/lchub")
            .build();

    }

    start() {
        this.client.start();
    }
}

export default new LCHub();