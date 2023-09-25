<template>
    <h3 style="margin-top:20px;">{{$t("message.devices")}}</h3>
    <div class="row">
        <div class="col-12">
            <div class="status_legend">
                <div class="status_legend">
                    {{$t("message.deviceStatusLegend")}}
                    <div class="status_legend"><span>{{$t("message.deviceLoaded")}}</span><span class="icon icon-lamp-normal"></span></div>
                    <div class="status_legend"><span>{{$t("message.deviceNew")}}</span><span class="icon icon-lamp-new"></span></div>
                    <div class="status_legend"><span>{{$t("message.deviceLoading")}}</span><span class="icon icon-lamp-loading"></span></div>
                    <div class="status_legend"><span>{{$t("message.deviceWarning")}}</span><span class="icon icon-lamp-warning"></span></div>
                    <div class="status_legend"><span>{{$t("message.deviceLost")}}</span><span class="icon icon-lamp-lost"></span></div>
                    <div class="status_legend" v-if="discovering"><span class="discoveryOn">{{ $t("message.deviceScanFlagTrue") }}</span></div>
                    <div class="status_legend" v-else><span class="discoveryOff">{{ $t("message.deviceScanFlagfalse") }}</span></div>
                    <div class="status_legend">
                        <span>
                            <Button icon="pi pi-check" @click="stopDiscovery" class="btn btn-success">
                                <i class="fa fa-check" aria-hidden="true"></i> &nbsp; {{$t('message.deviceStartStopScanButton')}}
                            </Button>
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div :key="polling" class="col-12">
            <loader :active="isLoading" :is-full-page="fullPage"></loader>
            <div v-if="treeView">
                <tree-table :value="devices.ToTreeTable" class=""
                            selectionMode="single" :expandedKeys="expandedKeys" :filters="filters" filterMode="lenient" @nodeSelect="onNodeSelect" :scrollable="true" scrollHeight="75vh">
                    <template #header>
                        <div style="display:flex;">
                            <div class="text-left" style="margin-right:42%;">
                                <span class="icon icon-table_rows-switch" @click="toggleViewToTable"></span>
                                <span class="icon icon-account_tree-switch" @click="toggleViewToTree"></span>
                            </div>
                            <div class="text-right" style="margin-left:42%;">
                                <div class="p-input-icon-left">
                                    <i class="pi pi-search"></i>
                                    <InputText v-model="filters['global']" placeholder="Global Search" size="50" />
                                </div>
                            </div>
                        </div>
                    </template>
                    <column field="name" header="" :expander="true" sortable="true" headerClass="text-center" bodyClass="text-center">
                        <template #header>
                            <span>{{$t('message.deviceName')}}</span>
                        </template>
                    </column>
                    <column field="label" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                        <template #header>
                            <span>{{$t('message.deviceLabel')}}</span>
                        </template>
                    </column>
                    <column field="deviceStatus" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                        <template #header>
                            <span>{{$t('message.deviceStatus')}}</span>
                        </template>
                        <template #body="slotProps">
                            <span :class="iconStatusSelector(slotProps.node.data.deviceStatus)"></span>
                        </template>
                    </column>
                    <column field="dmxAddress" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                        <template #header>
                            <span>{{$t('message.deviceDmxStartAddress')}}</span>
                        </template>
                    </column>
                    <column field="softwareVersionIdLabel" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                        <template #header>
                            <span>{{$t('message.deviceFirmwareVersion')}}</span>
                        </template>
                    </column>
                    <column field="DmxFootprint" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                        <template #header>
                            <span>{{$t('message.deviceFootprint')}}</span>
                        </template>
                    </column>
<!--                    <column field="IsInProject" header="" headerClass="text-center" bodyClass="text-center">
                        <template #header>
                            <span>{{$t('message.isInProject')}}</span>
                        </template>
                        <template #body="slotProps">
                            <span v-if="slotProps.node.data.IsInProject" class="pi pi-check"></span>
                            <span v-else class="pi pi-times"></span>
                        </template>
                    </column>-->
                </tree-table>
            </div>
            <div v-else>
                <DataTable :value="devices.OnlyRdmDevice" :paginator="true" :rows="10" :rowsPerPageOptions="[10,20,50]" v-model:filters="filtersDT"
                           :globalFilterFields="columns" selectionMode="single" @rowSelect="onNodeSelect" :scrollable="true" scrollHeight="75vh">
                    <template #header>
                        <div style="display:flex;">
                            <div class="text-left" style="margin-right:42%;">
                                <span class="icon icon-table_rows-switch" @click="toggleViewToTable"></span>
                                <span class="icon icon-account_tree-switch" @click="toggleViewToTree"></span>
                            </div>
                            <div class="text-right" style="margin-left:42%;">
                                <div class="p-input-icon-left">
                                    <i class="pi pi-search"></i>
                                    <InputText v-model="filters['global']" placeholder="Global Search" size="50" />
                                </div>
                            </div>
                        </div>
                    </template>
                    <column field="DeviceName" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                        <template #header>
                            <span>{{$t("message.deviceName")}}</span>
                        </template>
                    </column>
                    <column field="Label" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                        <template #header>
                            <span>{{$t("message.deviceLabel")}}</span>
                        </template>
                    </column>
                    <column field="DmxAddress" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                        <template #header>
                            <span>{{$t("message.deviceDmxStartAddress")}}</span>
                        </template>
                    </column>
                    <column field="SoftwareVersionIdLabel" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                        <template #header>
                            <span>{{$t("message.deviceFirmwareVersion")}}</span>
                        </template>
                    </column>
                    <column field="DmxFootprint" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                        <template #header>
                            <span>{{$t("message.deviceFootprint")}}</span>
                        </template>
                    </column>
                    <column field="IpAddress" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                        <template #header>
                            <span>{{$t("message.deviceIpAddress")}}</span>
                        </template>
                    </column>
                    <column field="ParentPort" header="" sortable="true" headerClass="text-center" bodyClass="text-center">
                        <template #header>
                            <span>{{$t("message.devicePortAddress")}}</span>
                        </template>
                    </column>
                </DataTable>
            </div>
        </div>

        <ModalDialog header="" v-model:visible="displayDeviceInfo" :breakpoints="{'960px': '75vw', '640px': '90vw'}" :style="{width: '50vw'}">
            <template #header>
                <span>{{$t("message.deviceInfoHeader")}} : {{this.selectedDevice.devName}}</span>
            </template>
            <device-info :device="selectedDevice" @paramsChanged="onParamsChanged" @higlitedLamp="onHiglightedLamp"
                         @dmxAddressChanged="onDmxChange" @labelChanged="onLabelChanged" @deviceDataReloaded="onDeviceDataReloaded"></device-info>
            <template #footer>
                <PrimButton label="Close" icon="pi pi-times" @click="closeDeviceInfo" class="p-button-text" />
            </template>
        </ModalDialog>
    </div>
</template>

<script>
import { HTTP } from '../../global/commonHttpRequest';
import DeviceInfo from './DeviceInfo.vue';
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import TreeTable from 'primevue/treetable/sfc';
import Column from 'primevue/column';
import Checkbox from 'primevue/checkbox';
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import DataTable from 'primevue/datatable';
import { FilterMatchMode, FilterOperator } from 'primevue/api';
export default {
    name: "Devices",
    components: {
        'device-info': DeviceInfo,
        'tree-table': TreeTable,
        'column': Column,
        'checkBox': Checkbox,
        'InputText': InputText,
        'PrimButton': Button,
        'ModalDialog': Dialog,
        'DataTable': DataTable,
    },
    data() {
        return {
            devices:null,
            polling: null,
            fullPage: false,
            isOpen: false,
            isOpenFirstChild: false,
            isOpenSecondChild: false,
            isOpenRdm: false,
            selectedDevice: null,
            currentlyActiveItem: null,
            discovering: null,
            connection: null,
            connFlag: false,
            parentComp: 'devicesComponent',
            treeView: true,
            expandedKeys: {},
            filters: {},
            filtersDT: {
                'global': { value: null, matchMode: FilterMatchMode.CONTAINS },
            },
            displayDeviceInfo: false,
            columns: ['DeviceName', 'Label', 'DmxAddress', 'SoftwareVersionIdLabel', 'DmxFootprint', 'IpAddress', 'ParentPort'],
        }
    },
    computed: {
        isFolder: function () {
            return this.devices.children && this.devices.children.length;
        },
        /*isLoading() {
            return this.devices.Id == '' ? true : false;
        },*/
        columnNameHeader () {
            return this.$t("message.deviceName");
        },
        columnLabelHeader () {
            return this.$t("message.deviceLabel");
        },
        columnStatusHeader () {
            return this.$t("message.deviceStatus");
        },
        columnDmxAddressHeader () {
            return this.$t("message.deviceDmxStartAddress");
        },
        columnFirmwareHeader () {
            return this.$t("message.deviceFirmwareVersion");
        },
        columnFootprintHeader () {
            return this.$t("message.deviceFootprint");
        },
        columnIsInProjectHeader () {
            return this.$t("message.isInProject");
        },
    },
    watch: {
        isLoading(newVal, oldVal) {
            return this.devices.Id == '' ? true : false;
        },
    },
    created() {
        setTimeout(() => {
            HTTP.get('/Configuration')
                .then(response => {
                    this.configValues = response.data;
                    console.log(this.configValues);
                    this.appBaseUrl = this.configValues.baseUrl;
                    if (this.configValues.newLogEntries > 0) {
                        $('#notifier').removeClass('no-notifiers');
                        $('#notifier').addClass('notified');
                        $('#notifier').text(this.configValues.newLogEntries);
                    }
                })
                .catch(e => {
                    console.log(e);
                });
            /*if (!this.configValues.isRdmEnabled) {*/
                HTTP.get('/Devices')
                    .then(response => {
                        console.log(response);
                        let arr = response.data;
                        this.devices = arr;
                        console.log(this.devices);
                        this.discovering = arr.DeviceScanning;

                        this.devices.ToTreeTable.forEach(function (item) {
                            this.expandedKeys[item.key] = true;
                            item.children.forEach(function (elem) {
                                this.expandedKeys[elem.key] = true;
                                elem.children.forEach(function (child) {
                                    this.expandedKeys[child.key] = true;
                                }, this);
                            }, this);
                        }, this);
                    })
                    .catch(e => {
                        console.log(e);
                    });
            //}
        }, 5);
        this.connection = new HubConnectionBuilder()
            .withUrl("/api/lchub")
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        this.connection.start().then(() => {
            this.connFlag = true;
        }).catch(err => { console.error(err.toString()) });
    },
    beforeDestroy() {
        clearInterval(this.polling)
    },
    mounted() {
        this.polling = setInterval(() => {
            if (!this.configValues.isRdmEnabled) {
                HTTP.get('/Devices')
                    .then(response => {
                        let arr = response.data;
                        this.devices = arr;
                        this.discovering = arr.DeviceScanning;

                        arr.ToTreeTable.forEach(function (item) {
                            this.expandedKeys[item.key] = true;
                            item.children.forEach(function (elem) {
                                this.expandedKeys[elem.key] = true;
                                elem.children.forEach(function (child) {
                                    this.expandedKeys[child.key] = true;
                                }, this);
                            }, this);
                        }, this);
                    })
                    .catch(e => {
                        console.log(e);
                    })
            }
        }, 10000);

        setTimeout(() => {
            this.selectedDevice = this.devicesProps;
        }, 500);

        this.connection.on('DeviceDataReloading', (newData) => {
            let that = this;
            that.selectedDevice = newData;
        });
        this.connection.on('DeviceDataReloadingError', (newData) => {
            let that = this;
            console.log(newData);
        });
    },
    methods: {
        selDeviceHandle(data) {
            console.log(data);
            Object.assign(this.selectedDevice, data);
            this.selectedItem = data;
        },
        stopDiscovery: function () {
            this.discovering = !this.discovering;
            let headers = { 'Content-Type': 'application/json' };
            let StartStopScheduler = { action: this.discovering };
            HTTP.post("/Devices/StartStopDiscovery", StartStopScheduler, { headers });
        },
        toggleViewToTable: function () {
            this.treeView = false;
            $(document).ready(function () {
                $('#tableViewDevicesRdm').DataTable();
            });
        },
        toggleViewToTree: function () {
            this.treeView = true;
        },
        iconStatusSelector: function (row) {
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
                    let logInfo = { 'level': level, 'item': item };
                    this.$emit('writeToLog', logInfo);
                    return 'folder icon icon-lamp-warning';
                    break;
                case 4:
                    let level2 = 'lostDevice';
                    let logInfoLost = { 'level': level2, 'item': item };
                    this.$emit('writeToLog', logInfoLost);
                    return 'folder icon icon-lamp-lost';
                    break;
            }
        },
        onNodeSelect: function (node) {
            switch (node.data.Type) {
                case 'ArtNetGateway':
                    this.selectedDevice = this.devices;
                    this.selectedDevice.devName = this.devices.LongName;
                    break;
                case 'ArtNetGatewayNode':
                    this.selectedDevice = this.devices.children.find(elem => elem.Id == node.data.Id);
                    this.selectedDevice.devName = this.selectedDevice.DisplayShortName;
                    break;
                case 'GatewayUniverse':
                    let universes = [];
                    this.devices.children.forEach(function (item) { item.children.forEach(function (elem) { universes.push(elem); }) });
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
                    this.selectedDevice = this.devices.OnlyRdmDevice.find(elem => elem.Id == node.data.Id);
                    this.selectedDevice.devName = this.selectedDevice.DeviceName;
                    break;
            }
            this.displayDeviceInfo = true;
        },
        closeDeviceInfo: function () {
            this.displayDeviceInfo = false;
        }
    }
}
</script>

<style scoped>

</style>