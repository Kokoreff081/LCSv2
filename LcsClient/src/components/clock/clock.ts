import {Component, Prop, Vue} from 'vue-facing-decorator';

@Component({
    name: 'Clock',
})
export default class Clock extends Vue {
    hours: number  = 0;
    minutes: string = "0";
    seconds: string = "0";

    setTime () {
        setInterval(() => {
            const date = new Date()
            this.hours = date.getHours()
            this.minutes = this.checkSingleDigit(date.getMinutes());
            this.seconds = this.checkSingleDigit(date.getSeconds());
        }, 1000)
    }
    checkSingleDigit (digit:number) {
        return ('0' + digit).slice(-2)
    }
    
    mounted(){
        this.setTime();
    }
}
