<template>
    <div class="row">
        <div class="col">
            <h3 class="rowPadd">{{$t("message.projectProjectScheduler")}}</h3>
            <div class="scheduler_scenario_time_div">
                <span class="scheduler_scenario_time_scenTime_span">{{scenarioTime}}</span>
                <span class="scheduler_scenario_time_span"> / </span>
                <span class="scheduler_scenario_time_span">{{scenarioRemainTime}}</span>
            </div>
            <div class="scheduler_scenario_buttons">
                <span class="icon" :class="prevNextSelector" @click="startPrevTask"></span>
                <span class="">
                    <Button @click="startStopAllTasks" class="btn btn-primary">{{$t('message.schedulerStartStop')}}</Button>
                </span>
                <span class="icon" :class="nextPrevSelector" @click="startNextTask"></span>
                <span class="icon icon-addTask" @click="addTask"></span>
                <span>
                    <myselect :options="schedulerFiles" v-model="selectedFile" @change="selectSchedulerFile()" optionLabel="name" optionValue="index" :value="useState" :editable="false">
                        <template #value="slotProps">
                            <div class="p-dropdown-code">
                                <span v-if="slotProps.value">{{ slotProps.value.name }}</span>
                                <span v-else>
                                    {{ slotProps.placeholder }}
                                </span>
                            </div>
                        </template>
                    </myselect>
                </span>
                <span style="margin-left:10px;">
                    <Button @click="manageSchedulerFiles" class="btn btn-success"><i class="fa fa-list"></i>&nbsp;{{$t('message.projectManageSchedulerFiles')}}</Button>
                </span>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-11 item bold">
            <div class="row">
                <div class="div-table-id">#</div>
                <div class="div-table">{{$t("message.schedulerScenario")}}</div>
                <div class="div-table">{{$t("message.schedulerDate")}}</div>
                <div class="div-table">{{$t("message.schedulerStart")}}</div>
                <div class="div-table">{{$t("message.schedulerRepeat")}}</div>
                <div class="div-table">{{$t("message.schedulerDuration")}}</div>
                <div class="div-table-id" v-if="taskChanged"></div>
            </div>
            <div class="row" v-show="hasTasks" v-for="(task, index) in schedulers">
                <div class="div-table-id scheduler_scenario_buttons">
                    <span><PrimButton icon="pi pi-trash" class="p-button-rounded p-button-secondary" @click="delTask(task.id)" /></span>
                    <span class="spanPrimeMargin"><PrimButton icon="pi pi-pencil" class="p-button-rounded p-button-secondary" @click="openModalDateChanged(task)" /></span>
                </div>
                <div class="div-table">
                    <span>{{task.scenarioName}}</span>
                </div>
                <div class="div-table">
                    <span v-if="task.selectedWeekDays == 0 && task.timeType == 0">{{task.startDate}}</span>
                    <span v-else><week-days :taskWeekDays="task.selectedWeekDays" :mode="repeating"></week-days></span>
                </div>
                <div class="div-table">
                    <span v-if="task.timeType == 0">{{task.startTime}}</span>
                    <span v-else-if="task.timeType == 1">{{task.startTime}} ({{$t("message.sunrize")}} + {{task.minutes}} {{$t("message.astronomicMinutes")}})</span>
                    <span v-else>{{task.startTime}} ({{$t("message.sunset")}} + {{task.minutes}} {{$t("message.astronomicMinutes")}})</span>
                </div>
                <div class="div-table"><span :class="loopedSelector(task)" @click="toggleLoop(task)"></span></div>
                <div class="div-table">{{task.duration}}</div>
                <div class="div-table-id" v-if="task.taskChanged"><span class="icon icon-save" @click="saveDateDayChagesInTask"></span></div>
            </div>
        </div>
    </div>

    <Dialog header="$t('message.schedulerAddTask')" v-model:visible="isModalVisible" :breakpoints="{'960px': '75vw', '640px': '90vw'}" :style="{width: '50vw'}" :modal="true">
        <div class="row item bold">
            <div class="col">{{$t("message.schedulerScenario")}}</div>
            <div class="col"></div>
            <div class="col">{{$t("message.schedulerDate")}}</div>
            <div class="col">{{$t("message.schedulerRepeat")}}</div>
            <div class="col">{{$t("message.schedulerDuration")}}</div>
        </div>
        <div class="row">
            <div class="col">
                <myselect :options="scenarios" v-model="selectedScenario" @change="onItemSelected" optionLabel="scenarioName" optionValue="scenarioId" :editable="false">
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
                    <Calendar selectionMode="range" :showTime="true" v-model="dateTimes"></Calendar>
                </div>
                <div v-else-if="repeatingModal == 'add'">
                    <week-days :taskWeekDays="newTask.selectedWeekDays" :mode="repeatingModal" @valueChanged="onDateValueChanged"></week-days>
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
            <div class="col div-table"><span :class="loopedSelector(newTask)" @click="toggleLoop(newTask)"></span></div>
            <div class="col div-table">{{selectedScenario.scenarioTime}}</div>
        </div>
        <template #footer>
            <PrimButton label="No" icon="pi pi-times" @click="closeModal" class="p-button-text" />
            <PrimButton label="Yes" icon="pi pi-check" @click="submit" />
        </template>
    </Dialog>

    <Dialog header="$t('message.schedulerEditTask')" v-model:visible="modalDateChangeVisibility">
        <div class="row item bold">
            <div class="col">{{$t("message.schedulerScenario")}}</div>
            <div class="col"></div>
            <div class="col">{{$t("message.schedulerDate")}}</div>
            <div class="col">{{$t("message.schedulerRepeat")}}</div>
            <div class="col">{{$t("message.schedulerDuration")}}</div>
        </div>
        <div class="row item bold">
            <div class="col">
                <myselect :options="scenarios" v-model="selectedScenario2" @change="onItemSelected2" optionLabel="scenarioName" optionValue="scenarioId" :editable="false">
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
                <input type="radio" id="one" value="dateTime" v-model="repeatingModal2" />
                <label for="one">DateTime (Range maybe)</label>
                <br />
                <input type="radio" id="two" value="add" v-model="repeatingModal2" />
                <label for="two">Select week days for repeat this scenario</label>
                <br />
                <input type="radio" id="three" value="astronomic" v-model="repeatingModal2" />
                <label for="three">Astronomic sunrize/sunset settings</label>

            </div>
            <div class="col div-table">
                <div v-if="repeatingModal2 == 'dateTime'">
                    <Calendar selectionMode="range" :showTime="true" v-model="dateTimes2"></Calendar>
                </div>
                <div v-else-if="repeatingModal2 == 'add'">
                    <week-days :taskWeekDays="taskToDateChanging.selectedWeekDays" :mode="repeatingModal2" @valueChanged="onDateValueChanged2"></week-days>
                </div>
                <div v-else>
                    <input type="radio" id="one" value="Sunrize" v-model="timeType2" />
                    <label for="one">Sunrize</label>
                    <br />
                    <input type="radio" id="two" value="Sunset" v-model="timeType2" />
                    <label for="two">Sunset</label>
                    <br />
                    <input type="number" v-model="minutes2" />
                </div>
            </div>
            <div class="col div-table"><span :class="loopedSelector(taskToDateChanging)" @click="toggleLoop(taskToDateChanging)"></span></div>
            <div class="col div-table">{{selectedScenario2.scenarioTime}}</div>
        </div>
        <template #footer>
            <PrimButton label="No" icon="pi pi-times" @click="closeModal2" class="p-button-text" />
            <PrimButton label="Yes" icon="pi pi-check" @click="submit2" />
        </template>
    </Dialog>

    <Dialog Header="" v-model:visible="isSchedulerFilesManagerVisible">
        <div>
            <div v-show="hasFiles" v-for="file in fileList">
                <div class="div-table-id scheduler_scenario_buttons">
                    <span class="icon icon-delTask" @click="delFile(file)"></span>
                    <span class="item" :class="{bold: file.isCurrentFile}">{{file.name}}</span>
                </div>
            </div>
            <div class="scheduler_scenario_buttons">
                <span><input type="text" v-model="fileName" /></span>
                <span class="icon" :class="classSelector" @click="addFile"></span>
            </div>
        </div>
        <template #footer>
            <PrimButton label="Close" icon="pi pi-times" @click="closeModalSFM" class="p-button-text" />
        </template>
    </Dialog>
</template>
<script>
    import Datepicker from '@vuepic/vue-datepicker';
    import WeekDaysComponentVue from './WeekDaysComponent.vue';
    import ModalSchedulerFileManager from './ManageSchedulerFilesModal.vue';
    import { HTTP } from '../../global/commonHttpRequest';
    import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
    import Button from 'primevue/button';
    import Dropdown from 'primevue/dropdown';
    import Dialog from 'primevue/dialog';
    import Calendar from 'primevue/calendar';

    export default {
        name: 'Scheduler',
        components: {
            'myselect': Dropdown,
            'datepicker': Datepicker,
            'week-days': WeekDaysComponentVue,
            'sheduler-files-manager': ModalSchedulerFileManager,
            'PrimButton': Button,
            'Dialog': Dialog,
            'Calendar': Calendar,
        },
        props: ['schedulers', 'scenarios', 'groups', 'initPlayTasks', 'scenarioToPlay', 'schedulerFiles'],
        mounted() {
            console.log(this.scenarios);
            setTimeout(() => {
                this.scenariosInner = this.scenarios;
                this.startStop = this.initPlayTasks;
                this.selectedFile = this.schedulerFiles.find(elem => elem.isCurrentFile == true);
                this.selectedScenario = this.scenarioToPlay;
                this.fileList = this.schedulerFiles;
            }, 1000);

            this.connection.on('NewFrame', (info) => {
                let that = this;
                that.scenarioTime = info.scenarioPlayedTime;
                that.scenarioRemainTime = info.scenarioTime;
            });
        },
        data: function () {
            return {
                addedScenarios: [],
                scenariosInner: null,
                scheduledScenarios: [],
                scenarioTime: '00:00:00',
                scenarioRemainTime: '00:00:00',
                scenario: null,
                tasks: this.schedulers,
                isModalVisible: false,
                modalDateChangeVisibility: false,
                repeating: 'view',
                taskToDateChanging: Object,
                weekDay: null,
                startStop: this.initPlayTasks,
                connFlag: false,
                connection: null,
                scenariosToSelect: [],
                selectedFile: Object,
                isSchedulerFilesManagerVisible: false,
                selectedScenario: this.scenarioToPlay,
                newTask: {
                    scenarioId: null,
                    scenarioName: null,
                    isLooped: false,
                    timeType: null,
                    minutes: null,
                    selectedWeekDays: null,
                    specifiedDateTime: null,
                    specifiedDateTimes: [],

                },
                timeType: 0,
                repeatingModal: 'dateTime',
                repeatingModal2: 'dateTime',
                dateTimes: null,
                dateTimes2: null,
                timeType2: 0,
                minutes: 0,
                minutes2: 0,
                selectedScenario2: null,
                weekDay2: 0,
                fileName: '',
                fileList:null//this.schedulerFiles,
            }
        },
        created() {
            this.connection = new HubConnectionBuilder()
                .withUrl("/api/lchub")
                .withAutomaticReconnect()
                .configureLogging(LogLevel.Information)
                .build();

            this.connection.start().then(() => {
                this.connFlag = true;
            }).catch(err => { console.error(err.toString()) });
            
        },
        
        
        computed: {
            hasTasks: function () {
                return this.schedulers && this.schedulers.length;
            },
            hasFiles: function () {
                return this.schedulerFiles && this.schedulerFiles.length;
            },
            classSelector: function () {
                if (this.fileName == '')
                    return 'icon-addTask-inactive';
                return 'icon-addTask';
            },
            weekDay: function () {
                switch (this.taskWeekDays) {
                    case 127://every day
                        return $t("message.weekDaysEveryday");
                        break;
                    case 31://work days
                        return $t("message.weekDaysWorkDays");
                        break;
                    case 96://weekend days
                        return $t("message.weekDaysWeekendDays");
                        break;
                    case 64:
                        return $t("message.weekDaysSunday");
                        break;
                    case 32:
                        return $t("message.weekDaysSaturday");
                        break;
                    case 16:
                        return $t("message.weekDaysFriday");
                        break;
                    case 8:
                        return $t("message.weekDaysThursday");
                        break;
                    case 4:
                        return $t("message.weekDayWednesday");
                        break;
                    case 2:
                        return $t("message.weekDaysTuesday");
                        break;
                    case 1:
                        return $t("message.weekDaysMonday");
                        break;
                }
            },
            prevNextSelector: function () {
                if (this.startStop) {
                    return 'icon-Rewind-active';
                }
                return 'icon-Rewind';
            },
            nextPrevSelector: function () {
                if (this.startStop) {
                    return 'icon-Forward-active';
                }
                return 'icon-Forward';
            },
        },
        watch: {
            startStop: function (oldVal, newVal) {
                let headers = { 'Content-Type': 'application/json' };
                let currentValue = this.startStop;
                console.log(this.startStop, currentValue);
                let StartStopScheduler = { action: currentValue };
                console.log("was:" + oldVal + "now:" + newVal);
                HTTP.post('Scheduler/StartStopAllTasks', StartStopScheduler, { headers });
            },
            scenariosInner: {
                handler() {
                    this.scenariosInner = this.scenarios;
                }, immediate: true
            },
            fileList: {
                handler() {
                    this.fileList = this.schedulerFiles;
                },
                immediate:true
            }
        },
        methods: {
            addTask: function () {
                this.isModalVisible = true;
            },
            onItemSelected: function (data) {
                let findedScen = this.scenarios.find(f => f.scenarioId == this.selectedScenario);
                this.selectedScenario = findedScen;
                this.scheduledScenarios.push(findedScen);
            },
            onItemSelected2: function (data) {
                let findedScen = this.scenarios.find(f => f.scenarioId == this.selectedScenario2);
                this.selectedScenario2 = findedScen;
                this.taskToDateChanging.scenarioId = findedScen.scenarioId;
                this.taskToDateChanging.scenarioName = findedScen.scenarioName;
                console.log(this.taskToDateChanging);
            },
            
            closeModal() {
                this.isModalVisible = false;
            },
            delTask: function (id) {
                console.log(id);
                let headers = { 'Content-Type': 'application/json' };
                let data = { taskId: id };
                HTTP.post('Scheduler/DelTask', data, { headers });
                setTimeout(() => { window.location.reload(); }, 1000);
            },
            onAddNewTask: function (data) {
                console.log(data);
                let ScheduleItemFront = {
                    IsLooped : data.isLooped,
                    SpecifiedDateTime : data.specifiedDateTime,
                    SpecifiedDateTimes : data.specifiedDateTimes,
                    ScenarioId: data.scenarioId,
                    ScenarioName: data.scenarioName,
                    SelectedWeekDays: data.selectedWeekDays,
                    Minutes : data.minutes,
                    TimeType: data.timeType,
                }
                console.log(ScheduleItemFront);
                let headers = { 'Content-Type': 'application/json' };
                HTTP.post('Scheduler/AddTask', ScheduleItemFront, { headers });
                this.isModalVisible = false;
            },
            openModalDateChanged: function (task) {
                console.log(task);
                this.taskToDateChanging = {
                    id: task.id,
                    scenarioId: task.scenarioId,
                    scenarioName: task.scenarioName,
                    isLooped: task.isLooped,
                    timeType: task.timeType,
                    minutes: task.minutes,
                    selectedWeekDays: task.selectedWeekDays,
                    specifiedDateTime: task.specifiedDateTime,
                    specifiedDateTimes: task.specifiedDateTimes,
                    taskChanged: task.taskChanged,
                };
                this.selectedScenario2 = this.scenarios.find(elem => elem.scenarioId == this.taskToDateChanging.scenarioId);
                this.modalDateChangeVisibility = true;
            },
            closeModal2() {
                this.modalDateChangeVisibility = false;
            },
            DateDayChanged: function (data) {
                if (!isNaN(data.value)) {
                    if (this.repeatingModal2 == 'astronomic') {
                        if (data.timeType == 'Sunrize')
                            this.taskToDateChanging.timeType = 1;
                        else
                            this.taskToDateChanging.timeType = 2;
                        this.taskToDateChanging.minutes = data.value;
                        this.taskToDateChanging.selectedWeekDays = 127;
                        this.taskToDateChanging.specifiedDateTimes = [];
                        this.taskToDateChanging.specifiedDateTime = new Date();
                    }
                    else {
                        this.taskToDateChanging.selectedWeekDays = data.value;
                        this.taskToDateChanging.timeType = data.timeType;
                        this.taskToDateChanging.specifiedDateTime = new Date();
                        this.taskToDateChanging.specifiedDateTimes = [];
                        this.taskToDateChanging.taskChanged = data.taskChanged;
                    }
                    
                }
                else if (data.value.length>1)
                {
                    if (data.value.find(elem => elem == null) != 'undefined')
                    {
                        if (this.taskToDateChanging.specifiedDateTimes != data.value)
                        {
                            this.taskToDateChanging.specifiedDateTimes = data.value;
                            this.taskToDateChanging.timeType = data.timeType;
                            this.taskToDateChanging.taskChanged = data.taskChanged;
                        }
                    }
                    else
                    {
                        if (this.taskToDateChanging.specifiedDateTime != data.value[0])
                        {
                            this.taskToDateChanging.timeType = data.timeType;
                            this.taskToDateChanging.taskChanged = data.taskChanged;
                        }
                    }
                    this.taskToDateChanging.specifiedDateTime = data.value[0];
                    this.taskToDateChanging.selectedWeekDays = 0;
                }
                else
                {
                    this.taskToDateChanging.timeType = data.timeType;
                    this.taskToDateChanging.minutes = data.value;
                    this.taskToDateChanging.taskChanged = data.taskChanged;
                }
                console.log(this.taskToDateChanging);
                this.saveDateDayChagesInTask();
            },
            saveDateDayChagesInTask: function () {
                let headers = { 'Content-Type': 'application/json' };
                let ScheduleItemFront = {
                    Id: this.taskToDateChanging.id,
                    IsLooped: this.taskToDateChanging.isLooped,
                    SpecifiedDateTime: this.taskToDateChanging.specifiedDateTime,
                    SpecifiedDateTimes: this.taskToDateChanging.specifiedDateTimes,
                    ScenarioId: this.taskToDateChanging.scenarioId,
                    ScenarioName: this.taskToDateChanging.scenarioName,
                    SelectedWeekDays: this.taskToDateChanging.selectedWeekDays,
                    Minutes: this.taskToDateChanging.minutes,
                    TimeType: this.taskToDateChanging.timeType,
                    Duration: this.selectedScenario2.scenarioTime,
                    TaskChanged:true,
                }
                HTTP.post('/Scheduler/UpdateTask', ScheduleItemFront, { headers });
                setTimeout(() => { window.location.reload(); }, 1000);
            },
            startPrevTask: function () {
                if (this.startStop) {
                    let headers = { 'Content-Type': 'application/json' };
                    HTTP.post('/Scheduler/StartPrevTask', { headers });
                }
            },
            startNextTask: function () {
                if (this.startStop) {
                    let headers = { 'Content-Type': 'application/json' };
                    HTTP.post('/Scheduler/StartNextTask', { headers });
                }
            },
            selectSchedulerFile: function () {
                console.log(this.selectedFile);
                let selectedFileFromSelect = this.fileList.find(elem => elem.index == this.selectedFile);
                this.fileList.forEach(function (item) { if (item.index != selectedFileFromSelect.index) { item.isCurrentFile = false; } })
                let ChangeSchedulerFileRequest = { Index: selectedFileFromSelect.index, Name: selectedFileFromSelect.name };
                let headers = { 'Content-Type': 'application/json' };
                HTTP.post('/Scheduler/ChangeSchedulerFile', ChangeSchedulerFileRequest, { headers });
                this.selectedFile = selectedFileFromSelect;
                this.selectedFile.isCurrentFile = true;
            },
            manageSchedulerFiles: function () {
                this.isSchedulerFilesManagerVisible = !this.isSchedulerFilesManagerVisible;
            },
            closeModalSFM: function () {
                this.isSchedulerFilesManagerVisible = !this.isSchedulerFilesManagerVisible;
                this.selectedFile = this.schedulerFiles.find(elem => elem.isCurrentFile == true);
            },
            startStopAllTasks: function () {
                this.startStop = !this.startStop;
                let headers = { 'Content-Type': 'application/json' };
                let currentValue = this.startStop;
                console.log(this.startStop, currentValue);
                let StartStopScheduler = { action: currentValue };
                console.log("was:" + oldVal + "now:" + newVal);
                HTTP.post('Scheduler/StartStopAllTasks', StartStopScheduler, { headers });
            },
            toggleLoop: function (task) {
                task.isLooped = !task.isLooped;
                return this.loopedSelector(task);
            },
            loopedSelector: function (task) {
                if (task.isLooped)
                    return 'icon icon-isLooped';
                return 'icon icon-isNoLooped';
            },
            submit: function () {
                console.log(this.scheduledScenarios[0]);
                if (this.scheduledScenarios.length == 0) {
                    this.scheduledScenarios.push(this.selectedScenario);
                }
                this.newTask.scenarioId = this.scheduledScenarios[0].scenarioId;
                this.newTask.scenarioName = this.scheduledScenarios[0].scenarioName;
                console.log(this.dateTimes);
                if (this.repeatingModal == 'dateTime') {
                    let element = this.dateTimes.find(elem => elem == null);
                    if (element != null) {
                        this.newTask.specifiedDateTimes = this.dateTimes;
                        this.newTask.specifiedDateTime = this.dateTimes[0];
                    }
                    else {
                        this.newTask.specifiedDateTime = this.dateTimes[0];
                        this.newTask.specifiedDateTimes = [];
                        console.log("not range date");
                    }
                    this.newTask.selectedWeekDays = 0;
                    this.newTask.timeType = 0;
                    this.newTask.minutes = 0;
                }
                else if (this.repeatingModal != 'astronomic') {
                    this.newTask.specifiedDateTime = this.dateTimes;
                    this.newTask.timeType = 0;
                    this.newTask.minutes = 0;
                }
                else {
                    let dateTime = new Date();
                    dateTime.setDate(dateTime.getDate() + 1);
                    this.newTask.specifiedDateTime = dateTime;
                    if (this.timeType == 'Sunrize')
                        this.newTask.timeType = 1;
                    else
                        this.newTask.timeType = 2;
                    this.newTask.minutes = this.minutes;
                    this.newTask.selectedWeekDays = 0;
                }
                console.log(this.newTask);
                this.isModalVisible = false;
                this.onAddNewTask(this.newTask);
            },
            onDateValueChanged: function (data) {
                console.log(data);
                this.newTask.selectedWeekDays = data.weekDayVal;
                this.dateTimes = data.timer;
            },
            onDateValueChanged2: function (data) {
                console.log(data);
                this.weekDay2 = data.weekDayVal;
                console.log(this.weekDay2);
                this.taskToDateChanging.selectedWeekDays = data.weekDayVal;
                this.dateTimes = data.timer;
            },
            submit2: function () {
                let data = {};
                if (this.repeatingModal2 == 'dateTime') {
                    let valueData = null;
                    if (this.dateTimes2 == null)
                        valueData = this.taskToDateChanging.selectedWeekDays;
                    else {
                        let arrDates = [this.dateTimes2[0], this.dateTimes2[1]];
                        console.log(arrDates);
                        valueData = arrDates;
                    }
                    data = { timeType: 0, value: valueData, taskChanged: true };
                    console.log(data);
                    this.DateDayChanged(data);
                }
                else if (this.repeatingModal2 == 'add') {
                    data = { timeType: 0, value: this.weekDay2, taskChanged: true };
                    console.log(data);
                    this.DateDayChanged(data);
                }
                else {
                    data = { timeType: this.timeType2, value: this.minutes2, taskChanged: true };
                    console.log(data);
                    this.DateDayChanged(data);
                }
                this.closeModal2();
            },
            delFile: function (file) {
                if (file.isCurrentFile) {
                    alert(this.$t("message.delSchedulerFileAlert"));
                    return;
                }
                let index = this.fileList.indexOf(file);
                if (index > -1)
                    this.fileList.splice(index, 1);
                let ChangeSchedulerFileRequest = { Index: file.index, Name: file.name };
                let headers = { 'Content-Type': 'application/json' };
                HTTP.post('/Scheduler/DelSchedulerFile', ChangeSchedulerFileRequest, { headers });
            },
            addFile: function () {
                if (this.fileName == '') {
                    alert(this.$t("message.schedulerFileNameEmpty"));
                    return;
                }
                let existFile = this.fileList.find(e => e.name === this.fileName + '.lctt');
                console.log(existFile);
                if (existFile) {
                    alert(this.$t("message.schedulerFileExists"));
                    return;
                }
                let headers = { 'Content-Type': 'application/json' };
                let NewSchedulerFileName = { FileName: this.fileName };
                HTTP.post('/Scheduler/CreateNewSchedulerFile', NewSchedulerFileName, { headers });
            }
        }
    }
</script>