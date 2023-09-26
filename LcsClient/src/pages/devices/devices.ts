import {Component, Vue} from "vue-facing-decorator";
import DeviceInfo from '@/components/device-info/device-info.vue';
import DataTable from 'primevue/datatable';
import TreeTable from 'primevue/treetable/TreeTable.vue';
import Column from 'primevue/column';
import Checkbox from 'primevue/checkbox';
import Dialog from 'primevue/dialog';
import instance from "@/utils/axios";
import store from '@/store/index';

@Component({
    components:{
        'device-info':DeviceInfo,
        'tree-table': TreeTable,
        'DataTable': DataTable,
        'column': Column,
        'checkBox': Checkbox,
        'ModalDialog': Dialog,
    }
})
export default class Devices extends Vue {
    public devices:any;
    public onlyRdmDevices:any;
    public onlyArtnetDevices:any;
    public discovering:boolean;
    public columns:any = ['DeviceName', 'Label', 'DmxAddress', 'SoftwareVersionIdLabel', 'DmxFootprint', 'IpAddress', 'ParentPort'];
    public displayDeviceInfo:boolean = false;
    public selectedDevice:any = null; 
    created(){
        let user = this.$store.state.auth.authentication.profile;
        if(user && user.role == 'user' || user.role=='admin') {
            let token = user.access_token;
            instance.get('/Devices')
                .then(response => {
                    this.devices = response.data;
                    this.discovering = response.data.DeviceScanning;
                    this.onlyArtnetDevices = response.data.Only
                    /*this.devices.ToTreeTable.forEach(function (item:any) {
                        TreeTable.expandedKeys[item.key] = true;
                        item.children.forEach(function (elem:any) {
                            this.expandedKeys[elem.key] = true;
                            elem.children.forEach(function (child:any) {
                                this.expandedKeys[child.key] = true;
                            }, this);
                        }, this);
                    }, this);*/
                })
                .catch(e => {
                    console.log(e);
                });
        }
    }
}