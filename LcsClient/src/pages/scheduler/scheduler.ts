import {Component, Vue} from "vue-facing-decorator";
import instance from "@/utils/axios";
import Button from 'primevue/button';
import Dropdown from 'primevue/dropdown';
import Dialog from 'primevue/dialog';
import Calendar from 'primevue/calendar';
import Checkbox from 'primevue/checkbox';
import Slider from 'primevue/slider';
import Clock from '@/components/clock/clock.vue'
// import Datepicker from '@vuepic/vue-datepicker';
import WeekDaysComponentVue from '@/components/WeekDaysComponent/WeekDaysComponent.vue';

@Component({
    components: {
        'myselect': Dropdown,
        // 'datepicker': Datepicker,
        'week-days': WeekDaysComponentVue,
        'PrimButton': Button,
        'Dialog': Dialog,
        'Calendar': Calendar,
        'Checkbox':Checkbox,
        'Slider':Slider,
        'clock': Clock,
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
    selectedScheduleInSelectedGroup:any = null;
    selectedScheduleItem:any = null;
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
            this.selectedScheduleGroup = this.scheduleGroups.find((item:any)=>item.IsCurrent === true);
            this.selectedScheduleInSelectedGroup = this.selectedScheduleGroup.Schedules.find((item:any) => item.IsSelected === true);
            console.log(this.selectedScheduleInSelectedGroup);
            if(this.selectedScheduleInSelectedGroup === undefined){
                console.log(this.selectedScheduleGroup.Schedules[0]);
                this.selectedScheduleGroup.Schedules[0].IsSelected = true;
                this.selectedScheduleInSelectedGroup = this.selectedScheduleGroup.Schedules.find((item:any) => item.IsSelected === true);
            }
            console.log(this.selectedScheduleGroup);
            this.selectedScheduleItem = this.selectedScheduleInSelectedGroup.ScheduleItems.find((item:any)=>item.IsSelected=== true);
            if(this.selectedScheduleItem === undefined){
                this.selectedScheduleInSelectedGroup.ScheduleItems[0].IsSelected = true;
                this.selectedScheduleItem = this.selectedScheduleInSelectedGroup.ScheduleItems.find((item:any)=>item.IsSelected=== true);
            }
        }
    }

    playActiveSelector() {
        if (this.startStop)
            return 'icon-playScenario-inactive';
        else
            return 'icon-playScenario';
    }
    
    selectedAndPlayingSelector(lcSchedule:any):string{
        console.log(lcSchedule);
        
        if(lcSchedule.IsCurrent || lcSchedule.IsSelected){
            if(lcSchedule.IsCurrent && !lcSchedule.IsSelected)
                return 'playingSchedule';
            else{
                return 'selectedSchedule';
            }
        }
        else
           return 'unselectedSchedule';
    }
    selectScheduleGroup(item:any){
        console.log(this.selectedScheduleGroup);
        console.log(item);
        item.IsCurrent = true;
        this.selectedScheduleGroup.IsCurrent = false;
        this.selectedScheduleGroup = item;
        let check = this.selectedScheduleGroup.Schedules.find((item:any) => item.IsSelected === true);
        this.selectedScheduleInSelectedGroup = check !== undefined ? check : this.selectedScheduleGroup.Schedules[0];
        
    }
    selectSchedule(item:any){
        console.log(this.selectedScheduleInSelectedGroup);
        console.log(item);
        item.IsSelected = true;
        this.selectedScheduleInSelectedGroup.IsSelected = false;
        this.selectedScheduleInSelectedGroup = item;
        let check = this.selectedScheduleInSelectedGroup.ScheduleItems.find((item:any) => item.IsSelected === true);
        this.selectedScheduleItem = check !== undefined ? check : this.selectedScheduleInSelectedGroup.ScheduleItems[0];
    }

    selectScheduleItem(item:any){
        console.log(this.selectedScheduleItem);
        console.log(item);
        item.IsSelected = true;
        this.selectedScheduleItem.IsSelected = false;
        this.selectedScheduleItem = item;
    }

    toggleLoop(task:any) {
        task.isLooped = !task.isLooped;
        task.taskChanged = true;
        return this.loopedSelector(task);
    }
    loopedSelector(task:any) {
        if (task.IsLooped)
            return 'icon icon-isLooped';
        return 'icon icon-isNoLooped';
    }
}