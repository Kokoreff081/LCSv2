<template>
    <table class="table" v-if="device && device.Type=='ArtNetGateway'">
        <tr>
            <td class="td_label">{{$t("message.deviceFirmwareVersion")}}</td>
            <td class="td_value">{{device.FirmwareVersion}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceOem")}}</td>
            <td class="td_value">{{device.Oem}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceUbeaVersion")}}</td>
            <td class="td_value">{{device.UbeaVersion}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceIsUbeaSupport")}}</td>
            <td class="td_value">{{device.UbeaVersion}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceIsRdmSupported")}}</td>
            <td class="td_value">{{device.UbeaVersion}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceRomBoot")}}</td>
            <td class="td_value">{{device.UbeaVersion}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceEstaCode")}}</td>
            <td class="td_value">{{device.EstaCode}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceNodeReport")}}</td>
            <td class="td_value">{{device.NodeReport}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceStyle")}}</td>
            <td class="td_value">{{device.Style}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceMacAddress")}}</td>
            <td class="td_value">{{device.MacAddress}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceIpAddress")}}</td>
            <td class="td_value">{{device.Id}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceWebSupport")}}</td>
            <td class="td_value">{{device.UbeaVersion}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceDhcpCapable")}}</td>
            <td class="td_value">{{device.UbeaVersion}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceArtNet3Capable")}}</td>
            <td class="td_value">{{device.UbeaVersion}}</td>
        </tr>
    </table>
    <table class="table" v-if="device && device.Type=='GatewayUniverse'">
        <tr>
            <td class="td_label">{{$t("message.deviceParentId")}}</td>
            <td class="td_value">{{device.ParentId}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceId")}}</td>
            <td class="td_value">{{device.Id}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceType")}}</td>
            <td class="td_value">{{device.Type}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.deviceStatus")}}</td>
            <td class="td_value">{{device.DeviceStatus}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.devicePortAddress")}}</td>
            <td class="td_value">{{device.PortAddress}}</td>
        </tr>
        <tr>
            <td class="td_label">{{$t("message.devicePortOutputStatus")}}</td>
            <td class="td_value">{{device.OutputStatus}}</td>
        </tr>
    </table>
</template>
<script>
export default {
    name: 'ArtNetInfo',
    props: {
        device: Object,
    },
    data: function () {
        return {
            selectedDevice: this.device,
        }
    },
    created() {
        console.log(this.selectedDevice);
    },
}
</script>