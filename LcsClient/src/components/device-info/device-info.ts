import {Component, Prop, Vue} from 'vue-facing-decorator';
import Sensors from '@/components/sensors/sensors.vue';
import instance from "@/utils/axios";
@Component({
    name: 'device-info',
    components:{'sensors':Sensors}
})
export default class Input extends Vue {
    @Prop() device: any;




    highlightLamp(id:string, item:any) {
        console.log(id, item);
        let user = this.$store.state.auth.authentication.profile;
        if(user && user.role == 'user' || user.role=='admin') {
            let token = user.access_token;
            let LampHighlight = {"id": id, "isOn": item.IsIdentifyOn};
            let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
            instance.post('/Devices/LampIdentity',
                LampHighlight, {headers})/*.then(response => {
                        let arr = JSON.parse(response.data);
                        this.$emit('highlight', arr);
                    })*/;
        }
    }
    changeLabel(id:string, label:string) {
        let user = this.$store.state.auth.authentication.profile;
        if(user && user.role == 'user' || user.role=='admin') {
            let token = user.access_token;
            let labelSent = {"id": id, "newLabel": label};
            let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
            instance.post('/Devices/ChangeLabel', labelSent, {headers})/*
                    .then(response => {
                        let arr = JSON.parse(response.data);
                        this.$emit('labelChanged', arr);
                    })*/;
        }

    }
    changeDmxAddress(id:string, dmxAddress:string) {
        let user = this.$store.state.auth.authentication.profile;
        if(user && user.role == 'user' || user.role=='admin') {
            let token = user.access_token;
            let newDmxAddress = {"id": id, "newDmxAddress": dmxAddress};
            let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
            instance.post('/Devices/ChangeDmxAddress', newDmxAddress, {headers})/*
                    .then(response => {
                        let arr = JSON.parse(response.data);
                        this.$emit('dmxAddressChanged', arr);
                    })*/;
        }
    }
    changeParam(item:any, id:string) {
        if (item.Value > item.MaxValidValue || item.Value < item.MinValidValue) {
            let message = "Error! Value must be between " + item.MinValidValue + " and " + item.MaxValidValue + " !";
            alert(message);
        }
        else {
            let user = this.$store.state.auth.authentication.profile;
            if(user && user.role == 'user' || user.role=='admin') {
                let token = user.access_token;
                let ParamToChange = {"id": id, "parameterId": item.ParameterId, "newValue": item.Value};
                console.log(ParamToChange);
                let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
                instance.post('/Devices/ChangeParameter', ParamToChange, {headers})
                    .then(response => {
                        let arr = JSON.parse(response.data);
                        this.$emit('paramsChanged', arr);
                    });
            }
        }
    }
    reloadDeviceData (id:string, item:any)
    {
        let DeviceReloadData :any = { "id": id};
        let headers = { 'Content-Type': 'application/json' };
        instance.post('/Devices/ReloadDeviceData', DeviceReloadData, { headers });
    }
}