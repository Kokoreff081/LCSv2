import {Component, Prop, Vue} from 'vue-facing-decorator';
import Sensors from '@/components/sensors/sensors.vue';
@Component({
    name: 'device-info',
    components:{'sensors':Sensors}
})
export default class Input extends Vue {
    @Prop() device: any;
    
}