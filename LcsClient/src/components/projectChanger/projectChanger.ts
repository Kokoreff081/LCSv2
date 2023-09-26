import {Component, Prop, Vue} from 'vue-facing-decorator';
import instance from '../../utils/axios';
import Input from '@/components/input/input.vue';

@Component({
    components: {
        'app-input': Input
    }
})
export default class ProjectChanger extends Vue {
    public currentFile : any = null;
    
}