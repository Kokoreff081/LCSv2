import {ref} from "vue";
import {Component, Vue} from "vue-facing-decorator";
import DeviceInfo from '@/components/device-info/device-info.vue';
import DataTable from 'primevue/datatable';
import InputText from 'primevue/inputtext';
import TreeTable from 'primevue/treetable';
import Column from 'primevue/column';
import Checkbox from 'primevue/checkbox';
import Dialog from 'primevue/dialog';
import instance from "@/utils/axios";
import store from '@/store/index';
import { FilterMatchMode, FilterOperator } from 'primevue/api';
import {IDevices} from '@/interfaces/Devices';
@Component({
    components:{
        'device-info':DeviceInfo,
        'Tree-table': TreeTable,
        'DataTable': DataTable,
        'Column': Column,
        'checkBox': Checkbox,
        'ModalDialog': Dialog,
        'InputText':InputText,
    }
})
export default class Devices extends Vue {
    devices:any[] = [];
    onlyRdmDevices:any[]= null;
    onlyArtnetDevices:any[]= null;
    onlyArtnetUniverses:any[]= null;
    toTreeTable:any[] = null;
    discovering:boolean = false;
    columns:any = ['DeviceName', 'Label', 'DmxAddress', 'SoftwareVersionIdLabel', 'DmxFootprint', 'IpAddress', 'ParentPort'];
    displayDeviceInfo:boolean = false;
    selectedDevice:any = null; 
    treeView:boolean = true;
    expandedKeys: any = [];
    filters: any = ref({}); 
    filtersDT: any = ref({
        'global': { value: null, matchMode: FilterMatchMode.CONTAINS },
    });
    filterMode: string = 'lenient';
    
    async mounted(){
        let user = this.$store.state.auth.authentication.profile;
        console.log(user);
        if(user && user.role == 'user' || user.role=='admin') {
            let token = user.access_token;
            console.log(token);
            let headers= {"Accept": "application/json", "Authorization": "Bearer " + token};
            console.log(headers);
            let response = await instance.get('/Devices/Index', {headers})
            //this.devices = response.data);
            let data = JSON.parse(response.data);
            this.devices = data;
            console.log(data);
            this.discovering = data.DeviceScanning;
            this.onlyArtnetDevices = data.OnlyArtNetControllers;
            this.onlyRdmDevices = data.OnlyRdmDevice;
            this.onlyArtnetUniverses = data.OnlyArtNetUniverses;
            this.toTreeTable = data.ToTreeTable;
            this.toTreeTable.forEach(function (item) {
                // @ts-ignore
                this.expandedKeys[item.key] = true;
                // @ts-ignore
                // @ts-ignore
                item.children.forEach(function (elem) {
                    // @ts-ignore
                    this.expandedKeys[elem.key] = true;
                    // @ts-ignore
                    elem.children.forEach(function (child) {
                        // @ts-ignore
                        this.expandedKeys[child.key] = true;
                    }, this);
                }, this);
            }, this);
        }
    }
    toggleViewToTable() {
        this.treeView = false;
        
    };
    toggleViewToTree() {
        this.treeView = true;
    };
    iconStatusSelector(row:any) {
        switch (row) {
            case 0:
                return 'folder icon icon-lamp-normal';
                break;
            case 1:
                return 'folder icon icon-lamp-new';
                break;
            case 2:
                return 'folder icon icon-lamp-loading';
                break;
            case 3:
                let level = 'warning';
                return 'folder icon icon-lamp-warning';
                break;
            case 4:
                let level2 = 'lostDevice';
                return 'folder icon icon-lamp-lost';
                break;
        }
    };
    onNodeSelect(node: any) {
        console.log(node);
        switch (node.data.Type) {
            case 'ArtNetGateway':
                let selected = this.onlyArtnetDevices.find((item) => item.Id === node.data.Id);
                this.selectedDevice = selected;
                this.selectedDevice.devName = node.data.DeviceName;
                break;
            case 'GatewayUniverse':
                console.log(node.data.Id);
                let selectedUniverse = this.onlyArtnetUniverses.find(elem => elem.Id === node.data.Id);
                this.selectedDevice = selectedUniverse;
                let port = '';
                if (this.selectedDevice.PortType == 128)
                    port = 'Output';
                else if (this.selectedDevice.PortType == 64)
                    port = 'Input';
                this.selectedDevice.devName = 'DMX ' + port + ' ' + this.selectedDevice.PortIndex + ' Address ' + this.selectedDevice.Universe;
                break;
            case 'RdmDevice':
                this.selectedDevice = this.devices.OnlyRdmDevice.find((elem:any) => elem.Id == node.data.Id);
                console.log(this.selectedDevice);
                this.selectedDevice.devName = this.selectedDevice.DeviceName;
                break;
        }
        this.displayDeviceInfo = true;
    };
    closeDeviceInfo() {
        this.displayDeviceInfo = false;
    };
    
    startStopDiscovery(){
        this.discovering = !this.discovering;
        let user = this.$store.state.auth.authentication.profile;
        let token = user.access_token;
        let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
        let StartStopScheduler = { action: this.discovering };
        instance.post("/Devices/StartStopDiscovery", StartStopScheduler, {headers});
    }
}