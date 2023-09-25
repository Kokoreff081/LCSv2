<template>
    <div class="clock-inner" :class="color">
        <div class="hour">{{hours}}</div>
        <div class="dots">:</div>
        <div class="min">{{minutes}}</div>
        <div class="dots">:</div>
        <div class="secs">{{seconds}}</div>
        <div class="mode"></div>
    </div>
</template>

<script>
export default {
    name: 'Clock',
    props: {
        color: {
            type: String,
            default: 'default'
        }
    },
    data () {
        return {
            hours: 0,
            minutes: 0,
            seconds: 0
        }
    },
    methods: {
        setTime () {
            setInterval(() => {
                const date = new Date()
                this.hours = date.getHours()
                this.minutes = this.checkSingleDigit(date.getMinutes())
                this.seconds = this.checkSingleDigit(date.getSeconds())
            }, 1000)
        },
        checkSingleDigit (digit) {
            return ('0' + digit).slice(-2)
        }
    },
    mounted () {
        this.setTime()
    }
}
</script>
