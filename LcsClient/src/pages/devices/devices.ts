
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
        'tree-table': TreeTable,
        'DataTable': DataTable,
        'column': Column,
        'checkBox': Checkbox,
        'ModalDialog': Dialog,
        'InputText':InputText,
    }
})
export default class Devices extends Vue {
    devices:IDevices[] = [];
    onlyRdmDevices:any[]= null;
    onlyArtnetDevices:any[]= null;
    onlyArtnetUniverses:any[]= null;
    ToTreeTable:any[] = null;
    discovering:boolean = false;
    columns:any = ['DeviceName', 'Label', 'DmxAddress', 'SoftwareVersionIdLabel', 'DmxFootprint', 'IpAddress', 'ParentPort'];
    displayDeviceInfo:boolean = false;
    selectedDevice:any = null; 
    treeView:boolean = true;
    expandedKeys: any = {};
    filtersDT: any = {
        'global': { value: '', matchMode: FilterMatchMode.CONTAINS },
    }
    
    async mounted(){
        let user = this.$store.state.auth.authentication.profile;
        console.log(user);
        if(user && user.role == 'user' || user.role=='admin') {
            let token = user.access_token;
            console.log(token);
            let headers= {"Accept": "application/json", "Authorization": "Bearer " + token};
            console.log(headers);
            let response = await instance.get('/Devices/Index', {headers})
            //this.devices = JSON.parse(response.data);
            let data = JSON.parse(response.data);
            this.devices = data;
            console.log(this.devices);
            this.discovering = data.DeviceScanning;
            this.onlyArtnetDevices = data.OnlyArtNetControllers;
            this.onlyRdmDevices = data.OnlyRdmDevice;
            this.onlyArtnetUniverses = data.OnlyArtNetUniverses;
            this.ToTreeTable = data.ToTreeTable;
            /*this.ToTreeTable.forEach(function (item) {
                console.log(item);
                this.expandedKeys[item.key] = true;
                item.children.forEach(function (elem) {
                    console.log(elem);
                    this.expandedKeys[elem.key] = true;
                    elem.children.forEach(function (child) {
                        console.log(child);
                        this.expandedKeys[child.key] = true;
                    }, this);
                }, this);
            }, this);*/
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
        switch (node.data.Type) {
            case 'ArtNetGateway':
                this.selectedDevice = this.devices;
                //this.selectedDevice.devName = this.devices.LongName;
                break;
            case 'ArtNetGatewayNode':
                //this.selectedDevice = this.devices.children.find((elem:any) => elem.Id == node.data.Id);
                this.selectedDevice.devName = this.selectedDevice.DisplayShortName;
                break;
            case 'GatewayUniverse':
                let universes:any[] = [];
                //this.devices.children.forEach(function (item:any) { item.children.forEach(function (elem:any) { universes.push(elem); }) });
                console.log(node.data.Id, universes);
                this.selectedDevice = universes.find(elem => elem.Id == node.data.Id);
                let port = '';
                if (this.selectedDevice.PortType == 128)
                    port = 'Output';
                else if (this.selectedDevice.PortType == 64)
                    port = 'Input';
                this.selectedDevice.devName = 'DMX ' + port + ' ' + this.selectedDevice.PortIndex + ' Address ' + this.selectedDevice.Universe;;
                break;
            case 'RdmDevice':
                //this.selectedDevice = this.devices.OnlyRdmDevice.find((elem:any) => elem.Id == node.data.Id);
                this.selectedDevice.devName = this.selectedDevice.DeviceName;
                break;
        }
        this.displayDeviceInfo = true;
    };
    closeDeviceInfo() {
        this.displayDeviceInfo = false;
    };
}