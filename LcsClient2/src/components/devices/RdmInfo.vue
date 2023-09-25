<template>
    <table class="table">
        <tr>
            <td class="td_value" colspan="2">
                <PrimButton :label="$t('message.deviceReload')" icon="pi pi-refresh" @click="reloadDeviceData(device.Id, device)" class="p-button-text" />
            </td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceLabel")}}</td>
            <td class="td_value"><input type="text" v-model="device.Label" @change="changeLabel(device.Id, device.Label)" class="rdmInput" /></td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceUid")}}</td>
            <td class="td_value">{{device.Id}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceIsIdentityOn")}}</td>
            <td class="td_value"><input type="checkbox" id="{{device.Id}}" v-model="device.IsIdentifyOn" @change="highlightLamp(device.Id, device)" class="rdmInput" /></td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceSoftwareVersionLabel")}}</td>
            <td class="td_value">{{device.SoftwareVersionIdLabel}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceDmxStartAddress")}}</td>
            <td class="td_value"><input type="text" v-model="device.DmxAddress" @change="changeDmxAddress(device.Id, device.DmxAddress)" class="rdmInput" /></td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceFootprint")}}</td>
            <td class="td_value">{{device.DmxFootprint}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceName")}}</td>
            <td class="td_value">{{device.DisplayName}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceSoftwareVersionId")}}</td>
            <td class="td_value">{{device.SoftwareVersionIdLabel}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceBootSoftwareVersionId")}}</td>
            <td class="td_value">{{device.BootSoftwareVersionId}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceBootSoftwareVersionLabel")}}</td>
            <td class="td_value">{{device.BootSoftwareVersionLabel}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceModelId")}}</td>
            <td class="td_value">{{device.DeviceModelId}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceManufacturer")}}</td>
            <td class="td_value">{{device.Manufacturer}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceModel")}}</td>
            <td class="td_value">{{device.Model}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.devicePowerCycles")}}</td>
            <td class="td_value">{{device.PowerCycles}}</td>
        </tr>
        <tr v-if="device.DeviceHours">
            <td class="td_label">{{$t("message.deviceHours")}}</td>
            <td class="td_value">{{device.DeviceHours}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceLampHours")}}</td>
            <td class="td_value">{{device.LampHours}}</td>
        </tr>
        <tr v-if="device.LampStrikes">
            <td class="td_label">{{$t("message.deviceLampStrikes")}}</td>
            <td class="td_value">{{device.LampStrikes}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceSensorsCount")}}</td>
            <td class="td_value">{{device.SensorCount}}</td>
        </tr>
        <tr v-if="device.SensorCount > 0">
            <td class="td_label">{{$t("message.deviceSensors")}}</td>
            <td style="float:right;">
                <sensors :sensors="sensorsArr"></sensors>
            </td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceSubDeviceCount")}}</td>
            <td class="td_value">{{device.SubDeviceCount}}</td>
        </tr>
        <tr v-if="device.PanInvert">
            <td class="td_label">{{$t("message.devicePanInvert")}}</td>
            <td class="td_value">{{device.PanInvert}}</td>
        </tr>
        <tr v-if="device.TiltInvert">
            <td class="td_label">{{$t("message.deviceTiltInvert")}}</td>
            <td class="td_value">{{device.TiltInvert}}</td>
        </tr>
        <tr v-if="device.PanTiltSwap">
            <td class="td_label">{{$t("message.devicePanTiltSwap")}}</td>
            <td class="td_value">{{device.PanTiltSwap}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceLastSeen")}}</td>
            <td class="td_value">{{new Date(device.LastSeen).toLocaleString()}}</td>
        </tr>
        <tr v-if="device.Parameters && device.Parameters.length">
            <td class="td_label">{{$t("message.deviceParameters")}}</td>
            <td style="float:right;">
                <table>
                    <tr>
                        <th class="td_value">{{$t("message.deviceParametersDescription")}}</th>
                        <th class="td_value">{{$t("message.deviceParametersCurValue")}}</th>
                    </tr>
                    <tr v-for="(item, index) in device.Parameters">
                        <td class="td_label">{{item.Description}}</td>
                        <td class="td_value"><input type="text" class="rdmInput" v-model="item.Value" @change="changeParam(item, device.Id)" /></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</template>
<script>
import { HTTP } from '../../global/commonHttpRequest';
import Sensors from './Sensors.vue';
import Button from 'primevue/button';

export default {
    name: 'RdmInfo',
    props: {
        device: Object,
    },
    components: {
        'sensors': Sensors,
        'PrimButton': Button,
    },
    emits: ['paramsChanged', 'highlight', 'dmxAddressChanged', 'labelChanged', 'dataReloaded'],
    data: function () {
        return {
            selectedDevice: this.device,
            sensorsArr: this.device.Sensors,
        }
    },
    created() {
        console.log(this.selectedDevice);
    },
    mounted() {
        console.log(this.device);
        setTimeout(() => {
            Object.assign(this.selectedDevice, this.device);
            this.sensorsArr = this.device.Sensors;
        }, 500);
    },
    watch: {

    },
    methods: {
        highlightLamp(id, item) {
            console.log(id, item);
            let LampHighlight = { "id": id, "isOn": item.IsIdentifyOn };
            let headers = { 'Content-Type': 'application/json' };
            HTTP.post('/Devices/LampIdentity',
                LampHighlight, { headers })/*.then(response => {
                        let arr = JSON.parse(response.data);
                        this.$emit('highlight', arr);
                    })*/;
        },
        changeLabel(id, label) {
            let labelSent = { "id": id, "newLabel": label };
            let headers = { 'Content-Type': 'application/json' };
            HTTP.post('/Devices/ChangeLabel', labelSent, { headers })/*
                    .then(response => {
                        let arr = JSON.parse(response.data);
                        this.$emit('labelChanged', arr);
                    })*/;

        },
        changeDmxAddress(id, dmxAddress) {
            let newDmxAddress = { "id": id, "newDmxAddress": dmxAddress };
            let headers = { 'Content-Type': 'application/json' };
            HTTP.post('/Devices/ChangeDmxAddress', newDmxAddress, { headers })/*
                    .then(response => {
                        let arr = JSON.parse(response.data);
                        this.$emit('dmxAddressChanged', arr);
                    })*/;
        },
        changeParam(item, id) {
            if (item.Value > item.MaxValidValue || item.Value < item.MinValidValue) {
                let message = "Error! Value must be between " + item.MinValidValue + " and " + item.MaxValidValue + " !";
                alert(message);
            }
            else {
                let ParamToChange = { "id": id, "parameterId": item.ParameterId, "newValue": item.Value };
                console.log(ParamToChange);
                let headers = { 'Content-Type': 'application/json' };
                HTTP.post('/Devices/ChangeParameter', ParamToChange, { headers })
                    .then(response => {
                        let arr = JSON.parse(response.data);
                        this.$emit('paramsChanged', arr);
                    });
            }
        },
        reloadDeviceData: function (id, item) {
            let DeviceReloadData = { "id": id};
            let headers = { 'Content-Type': 'application/json' };
            HTTP.post('/Devices/ReloadDeviceData',
                DeviceReloadData, { headers });
        }
    },
}
</script>