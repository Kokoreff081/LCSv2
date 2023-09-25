<template>
    <table>
        <tr>
            <th class="td_value">{{$t("message.deviceSensorNumber")}}</th>
            <th class="td_value">{{$t("message.deviceSensorDescription")}}</th>
            <th class="td_value">{{$t("message.deviceSensorCurValue")}}</th>
        </tr>
        <tr v-for="item in sensors">
            <td class="td_label">{{item.SensorNumber}}</td>
            <td class="td_value">{{item.Description}}</td>
            <td class="td_value" :class="{warning: item.PresentValueIsOutOfNormalRange}">{{item.SensorValue}}</td>
        </tr>
    </table>
</template>
<script>
export default {
    name: 'Sensors',
    props: {
        sensors: Array,
    },
    data: function () {
        return {
            sensorsArray: this.sensors,
        }
    },
}
</script>