﻿import {Component, Prop, Vue} from "vue-facing-decorator";
import Calendar from 'primevue/calendar';


@Component({
    components: {'Calendar': Calendar}
})

export default class WeekDaysComponent extends Vue {
    @Prop
    taskWeekDays:number = null;
    @Prop
    mode:string = null;

    weekDays :any[] = [
        { Name: "Monday", Value: 1, isChecked:false },
        { Name: "Tuesday", Value: 2, isChecked: false },
        { Name: "Wednesday", Value: 4, isChecked: false },
        { Name: "Thursday", Value: 8, isChecked: false },
        { Name: "Friday", Value: 16, isChecked: false },
        { Name: "Saturday", Value: 32, isChecked: false },
        { Name: "Sunday", Value: 64, isChecked: false },
        { Name: "WeekendDays", Value: 96, isChecked: false },
        { Name: "WorkDays", Value: 31, isChecked: false },
        { Name: "EveryDay", Value: 127, isChecked: false },
    ];
    taskWeekDay:any = null;
    weekDayValue:any = null;
    time:any = null;
    
    async mounted(){
        if (this.taskWeekDays != 0) {
            if (this.mode == 'add') {
                switch (this.taskWeekDays) {
                    case 127://every day
                        this.weekDays.forEach(item => { item.isChecked = true; });
                        break;
                    case 31://work days
                        this.weekDays.forEach(item => { if (item.Value <= 16) item.isChecked = true; else item.isChecked = false; });
                        break;
                    case 96://weekend days
                        this.weekDays.forEach(item => { if (item.Value >= 16 && item.Value <= 64) item.isChecked = true; else item.isChecked = false; });
                        break;
                    default:
                        this.weekDays.forEach(item => {
                            if (item.Value == this.taskWeekDays) item.isChecked = true;
                        });
                }
            }
            else {
                switch (this.taskWeekDays) {
                    case 127://every day
                        this.taskWeekDay = "Everyday";
                        break;
                    case 31://work days
                        this.taskWeekDay = "WorkDays";
                        break;
                    case 96://weekend days
                        this.taskWeekDay = "WeekendDays";
                        break;
                    case 64:
                        this.taskWeekDay = "Sunday";
                        break;
                    case 32:
                        this.taskWeekDay = "Saturday";
                        break;
                    case 16:
                        this.taskWeekDay = "Friday";
                        break;
                    case 8:
                        this.taskWeekDay = "Thursday";
                        break;
                    case 4:
                        this.taskWeekDay = "Wednesday";
                        break;
                    case 2:
                        this.taskWeekDay = "Tuesday";
                        break;
                    case 1:
                        this.taskWeekDay = "Monday";
                        break;
                }
            }
        }
    }

    checkboxChanged (item:any) {
        console.log(item.Value);
        switch (item.Value) {
            case 127://every day
                this.weekDays.forEach(item => { item.isChecked = !item.isChecked; });
                this.weekDayValue = item.Value;
            break;
            case 31://work days
                this.weekDays.forEach(item => { if (item.Value <= 16 && item.Value != 31) item.isChecked = true; else item.isChecked = false; });
                this.weekDayValue = item.Value;
            break;
            case 96://weekend days
                this.weekDays.forEach(item => { if (item.Value > 16 && item.Value <= 64) item.isChecked = true; else item.isChecked = false; });
                this.weekDayValue = item.Value;
            break;
            default:
                this.weekDays.forEach(item => {
                    if (item.Value == this.taskWeekDays) item.isChecked = true;
                    });
                this.weekDayValue += item.Value;
            break;
        }
        console.log(this.weekDayValue, this.time);
        let newDate = new Date(this.time);
        console.log(newDate.getHours());
        let dateTime = new Date();
        dateTime.setDate(dateTime.getDate() + 1);
        dateTime.setHours(newDate.getHours());
        dateTime.setMinutes(newDate.getMinutes());
        dateTime.setSeconds(0);
        console.log({ weekDayVal: this.weekDayValue, timer: dateTime });
        this.$emit('valueChanged', { weekDayVal: this.weekDayValue, timer: dateTime});
    }
}