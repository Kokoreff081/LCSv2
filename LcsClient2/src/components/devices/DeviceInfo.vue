<template>
    <artnetinfo v-if="device && (device.Type=='ArtNetGateway' || device.Type=='ArtNetGatewayNode' || device.Type=='GatewayUniverse')"
                :device="device"></artnetinfo>
    <rdminfo  v-if="device && device.Type=='RdmDevice'"
              :device="device" :paramsChanged="changeParam" :highlight="lampHighlighted" :dmxAddressChanged="changeDmxAddr" :labelChanged="labelChanged"></rdminfo>
</template>
<script>
import ArtNetInfo from './ArtNetInfo.vue';
import RdmInfo from './RdmInfo.vue';
export default {
    name: 'DeviceInfo',
    props: {
        device: Object,
    },
    components: {
        'artnetinfo': ArtNetInfo,
        'rdminfo': RdmInfo,
    },
    emits: ['paramsChanged', 'higlitedLamp', 'dmxAddressChanged', 'labelChanged', 'deviceDataReloaded'],
    data: function () {
        return {
            selectedDevice: this.device,
        }
    },
    created() {
        console.log(this.selectedDevice);
    },
    mounted() {
        console.log(this.device);
        setTimeout(() => {
            Object.assign(this.selectedDevice,this.device);
        }, 500);
    },
    watch: {
        selectedDevice(oldVal, newVal) {
            this.selectedDevice = newVal;
        }
    },
    methods: {
        changeParam(data) {
            this.$emit('paramsChanged', data);
        },
        lampHighlighted(data) {
            this.$emit('higlitedLamp', data);
        },
        changeDmxAddr(data) {
            this.$emit('dmxAddressChanged', data);
        },
        labelChanged(data) {
            this.$emit('labelChanged', data);
        },

    },
}
</script>