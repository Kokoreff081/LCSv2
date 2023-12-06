import {Component, Vue} from "vue-facing-decorator";
import instance from "@/utils/axios";
import Button from 'primevue/button';
import Dropdown from 'primevue/dropdown';
import Dialog from 'primevue/dialog';
import Calendar from 'primevue/calendar';
import Checkbox from 'primevue/checkbox';
import Slider from 'primevue/slider';
// import Datepicker from '@vuepic/vue-datepicker';
import WeekDaysComponentVue from '@/components/WeekDaysComponent/WeekDaysComponent.vue';
import {bool} from "yup";
@Component({
    components: {
        'myselect': Dropdown,
        // 'datepicker': Datepicker,
        'week-days': WeekDaysComponentVue,
        'PrimButton': Button,
        'Dialog': Dialog,
        'Calendar': Calendar,
        'Checkbox':Checkbox,
        'Slider':Slider
    }
})

export default class Scheduler extends Vue {
    addedScenarios:any[] =  [];
    scenariosInner:any = null;
    scheduledScenarios:any[] = [];
    scenarioTime:string = '00:00:00';
    scenarioRemainTime:string = '00:00:00';
    scenario:any = null;
    scheduleGroups:any[] = [];
    selectedScheduleGroup:any = null;
    isModalVisible:boolean = false;
    modalDateChangeVisibility:boolean = false;
    repeating:string = 'view';
    taskToDateChanging:any = null;
    weekDay:number = null;
    startStop:boolean = false;
    connFlag:boolean = false;
    connection:any = null;
    scenariosToSelect:any[] = [];
    selectedFile:any = null;
    selectedScenario:any = null;
    newTask:any = {
        scenarioId: null,
        scenarioName: null,
        isLooped: false,
        timeType: null,
        minutes: null,
        selectedWeekDays: null,
        specifiedDateTime: null,
        specifiedDateTimes: [],
    };
    timeType:number = 0;
    repeatingModal:string =  'dateTime';
    repeatingModal2:string = 'dateTime';
    dateTimes:any = null;
    dateTimes2:any = null;
    timeType2:number = 0;
    minutes:number = 0;
    minutes2:number = 0;
    selectedScenario2:any = null;
    weekDay2:number = 0;
    
    async mounted(){
        let user = this.$store.state.auth.authentication.profile;
        if(user && user.role == 'user' || user.role=='admin') {
            let token = user.access_token;
            console.log(token);
            let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
            let response = await instance.get('/Scheduler/Index', {headers});
            let data = JSON.parse(response.data);
            //console.log(data);
            this.scheduleGroups = data.Schedule;
            console.log(this.scheduleGroups);
            this.selectedScheduleGroup = this.scheduleGroups.find((item:any)=>item.IsCurrent === true);
        }
    }

    playActiveSelector() {
        if (this.startStop)
            return 'icon-playScenario-inactive';
        else
            return 'icon-playScenario';
    }
}