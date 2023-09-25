<template>
    <Chart :size="{ width: 500, height: 400 }"
           :data="data"
           :margin="margin"
           :direction="direction">

        <template #layers>
            <Grid strokeDasharray="2,2" />
            <Line :dataKeys="['name', 'pl']" />
        </template>

    </Chart>
</template>
<script>
    import { Chart, Grid, Line } from 'vue3-charts';
    import { defineComponent, ref } from 'vue';
    import SelectComponent from '../SelectComponent.vue';
    import { HTTP } from '../../global/commonHttpRequest';

    export default {
        name: 'Graphic',
        components: { Chart, Grid, Line, SelectComponent, },
        data: function () {
            return {
                devicesWithSensors: null,
                groups: null,
            }
        },
        mounted() {
            let smth = setTimeout(
                () => {
                    HTTP.get('/Devices/GetSensorsLamps').then(response => {
                        let arr = response.data;
                        this.devicesWithSensors = arr.devices;
                        for (let i = 0; i < arr.length; i++) {
                            let item = arr[i];
                            //this.groups.push({id:item})
                        }
                    }).catch(e => {
                        console.log(e);
                    });
                }, 500);
        },
    }
    

</script>