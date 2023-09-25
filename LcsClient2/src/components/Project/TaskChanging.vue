<template>
    <div class="row item bold">
        <div class="col">{{$t("message.schedulerScenario")}}</div>
        <div class="col"></div>
        <div class="col">{{$t("message.schedulerDate")}}</div>
        <div class="col">{{$t("message.schedulerRepeat")}}</div>
        <div class="col">{{$t("message.schedulerDuration")}}</div>
    </div>
    <div class="row">
        <div class="col">
            <myselect :options="scenariosToOptions" v-model="selectedScenario" @change="onItemSelected" optionLabel="scenarioName" optionValue="scenarioId" :value="useState" :editable="false">
                <template #value="slotProps">
                    <div class="p-dropdown-code">
                        <span v-if="slotProps.value">{{ slotProps.value.scenarioName }}</span>
                        <span v-else>
                            {{ slotProps.placeholder }}
                        </span>
                    </div>
                </template>
            </myselect>
        </div>
        <div class="col div-table">
            <input type="radio" id="one" value="dateTime" v-model="repeatingModal" />
            <label for="one">DateTime (Range maybe)</label>
            <br />
            <input type="radio" id="two" value="add" v-model="repeatingModal" />
            <label for="two">Select week days for repeat this scenario</label>
            <br />
            <input type="radio" id="three" value="astronomic" v-model="repeatingModal" />
            <label for="three">Astronomic sunrize/sunset settings</label>
        </div>
        <div class="col div-table">
            <div v-if="repeatingModal == 'dateTime'">
                <Calendar selectionMode="range" :showTime="true" v-model="dateTimes"></Calendar><br />
                <!--<div v-if="task.specifiedDateTimes != null && task.specifiedDateTimes.length>1"><span v-if="task.specifiedDateTimes[0]!=null">{{dateTimes[0]}}</span><span v-if="task.specifiedDateTimes[1]!=null">{{dateTimes[1]}}</span></div>
    <div v-else><span v-if="task.specifiedDateTime!=null">{{task.specifiedDateTime}}</span></div>-->
            </div>
            <div v-else-if="repeatingModal == 'add'">
                <week-days :taskWeekDays="task.selectedWeekDays" :mode="repeatingModal" @valueChanged="onDateValueChanged"></week-days>
            </div>
            <div v-else>
                <input type="radio" id="one" value="Sunrize" v-model="timeType" />
                <label for="one">Sunrize</label>
                <br />
                <input type="radio" id="two" value="Sunset" v-model="timeType" />
                <label for="two">Sunset</label>
                <br />
                <input type="number" v-model="minutes" />
            </div>
        </div>
        <div class="col div-table"><span :class="loopedClass" @click="toggleLoop"></span></div>
        <div class="col div-table">{{scenarioTime}}</div>
    </div>
    <div class="row">
        <PrimButton label="Cancel" icon="pi pi-times" @click="close" class="p-button-text" />
        <PrimButton label="Save" icon="pi pi-check" @click="submit" />
    </div>
</template>

<script>
    import Datepicker from '@vuepic/vue-datepicker';
    import WeekDaysComponentVue from './WeekDaysComponent.vue';
    import Clock from '../Clock.vue';
    import Dropdown from 'primevue/dropdown';
    import Calendar from 'primevue/calendar';
    import Button from 'primevue/button';
    export default {
        inject: ['dialogRef'],
        name: 'TaskChanging',
        components: {
            'datepicker': Datepicker,
            'week-days': WeekDaysComponentVue,
            'clock': Clock,
            'myselect': Dropdown,
            'Calendar': Calendar,
            'PrimButton': Button,
        },
        emits: ['close', 'submitAndClose'],
        data: function () {
            return {
                repeatingModal: 'dateTime',
                dateTimes: null,
                weekDay: 0,
                task: null,
                minutes: null,
                timeType: null,
                isNewTask: null,
                scenariosToOptions: null,
                selectedScenario: null,
                loopedClass: '',
                scenarioTime: null,
                scheduledScenarios: [],
            }
        },
        mounted() {
            //console.log(this.inputTask);
            /*setTimeout(() => {
                console.log(this.inputTask);
                this.task = this.inputTask; }, 1000);*/
            const params = this.dialogRef.data;
            console.log(params);
            this.task = {
                id: params.inputTask.id,
                scenarioId: params.inputTask.scenarioId,
                scenarioName: params.inputTask.scenarioName,
                isLooped: params.inputTask.isLooped,
                timeType: params.inputTask.timeType,
                minutes: params.inputTask.minutes,
                selectedWeekDays: params.inputTask.selectedWeekDays,
                specifiedDateTime: params.inputTask.specifiedDateTime,
                specifiedDateTimes: params.inputTask.specifiedDateTimes,
                taskChanged: params.inputTask.taskChanged,
            };
            this.loopedClass = this.task.isLooped ? 'icon icon-isLooped' : 'icon icon-isNoLooped';
            console.log(this.task);
            this.minutes = params.inputTask.minutes;
            this.timeType = params.inputTask.timeType;
            this.scenariosToOptions = params.scenarios;
            this.isNewTask = params.isNewTask;
            this.selectedScenario = params.scenarioInTask;
            this.scenarioTime = this.selectedScenario.scenarioTime;
            this.dateTimes = this.task.specifiedDateTimes;

        },
        methods: {
            close() {
                this.dialogRef.close();
            },
            submit: function () {
                let data = {};
                if (this.repeating == 'dateTime') {
                    data = { timeType: 0, value: this.dateTimes, taskChanged: true };
                    this.$emit('submitAndClose', data);
                }
                else if (this.repeating == 'add') {
                    data = { timeType: 0, value: this.weekDay, taskChanged: true };
                    this.$emit('submitAndClose', data);
                }
                else {
                    data = { tiemType: this.timeType, value: this.minutes, taskChanged: true };
                    this.$emit('submitAndClose', data);
                }

            },
            toggleLoop: function () {
                this.task.isLooped = !this.task.isLooped;
                return this.loopedSelector(task);
            },
            loopedSelector: function () {
                if (this.task.isLooped)
                    return 'icon icon-isLooped';
                return 'icon icon-isNoLooped';
            },
            onDateValueChanged: function (data) {
                this.weekDay = data;
            },
            onItemSelected: function (data) {

                let findedScen = this.scenarios.find(f => f.scenarioId == this.selectedScenario);
                this.selectedScenario = findedScen;
                console.log(findedScen);
                this.scheduledScenarios.push(findedScen);
            },
        },
    };
</script>