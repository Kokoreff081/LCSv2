import {Component, Vue} from "vue-facing-decorator";
import slider from "vue3-slider";
import Button from 'primevue/button';
import Dropdown from 'primevue/dropdown';
import instance from "@/utils/axios";
import Konva from "konva";

@Component({
    components: {
        'vue3-slider': slider,
        'myselect': Dropdown,
        'PrimButton': Button,
    }
})

export default class Scenarios extends Vue {
    frameToRender:any = null;
    connection:any = null;
    connFlag: boolean = false;
    configKonva:any = {
        width: 1900,
        height: 800,
    };
    groups:any[] =  [{ id: 0, name: 'All rasters' }];
    selected:any =  { id: 0, name: 'All rasters' };
    defaultSelected:any =  { id: 0, name: 'All rasters' };
    renderRasters: any = null;
    scenarios:any[] = null;
    stage:Konva.Stage =  null;
    startStopAnim:boolean = true;
    scenarioTicks:number =  0;
    selectedScenario: any =  null;
    selectedScen: number = null;
    playScen:any = null;
    isPlaying:boolean = false;
    playScenarioId:number =  -1;
    sliderVal:number =  0;
    sliderMinVal:number =  0;
    sliderMaxVal:number =  1;
    tooltipShow:boolean =  true;
    scaleBy:number =  1.01;
    xScale:number =   1;
    yScale:number =   1;
    
    
    async mounted(){
        let user = this.$store.state.auth.authentication.profile;
        if(user && user.role == 'user' || user.role=='admin') {
            let token = user.access_token;
            console.log(token);
            let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
            let response = await instance.get('/Renderer/Index', {headers});
            let data = JSON.parse(response.data);
            console.log(data);
            data.Rasters.forEach((elem:any) => this.groups.push({ id: elem.Id, name: elem.Name }));
            this.renderRasters = data.Rasters;
            this.scenarios = data.Scenarios;
            this.selectedScenario = this.scenarios[0];
            this.drawRastersInitial(this.renderRasters);
        }
    }
    onSliderValChanged() {
        console.log(this.sliderVal);
        let rws = { tick: this.sliderVal, scenarioId: this.playScen.scenarioId };
        let headers= {"Accept": "application/json", "Authorization": "Bearer " + token};
        instance.post('/Renderer/RewindScenario', rws, { headers });
    }
    drawRastersInitial(rasters:any){
        console.log(rasters);
        this.stage = new Konva.Stage({
            container: 'canvas',
            width: this.configKonva.width,
            height: this.configKonva.height,
            draggable: true,
            
        });
        this.stage.on('wheel', (e:any) => {
            // stop default scrolling
            e.evt.preventDefault();
            console.log(e.evt.deltaY);
            var oldScale = this.stage.scaleX();
            var pointer = this.stage.getPointerPosition();

            var mousePointTo = {
                x: (pointer.x - this.stage.x()) / oldScale,
                y: (pointer.y - this.stage.y()) / oldScale,
            };

            // how to scale? Zoom in? Or zoom out?
            let direction = e.evt.deltaY > 0 ? -1 : 1;

            // when we zoom on trackpad, e.evt.ctrlKey is true
            // in that case lets revert direction
            if (e.evt.ctrlKey) {
                direction = -direction;
            }

            var newScale = direction > 0 ? oldScale * this.scaleBy : oldScale / this.scaleBy;

            this.stage.scale({ x: newScale, y: newScale });

            var newPos = {
                x: pointer.x - mousePointTo.x * newScale,
                y: pointer.y - mousePointTo.y * newScale,
            };
            this.stage.position(newPos);
        });
        var Layer = new Konva.Layer();
        let totalWidth = 0;
        let totalHeight = 0;
        let xPosGroup = rasters[0].Id;
        for (let i = 0; i < rasters.length; i++) {
            let raster = rasters[i];
            if (raster.DimensionX * 30 > totalWidth)
                totalWidth += raster.DimensionX * 30;
            if (raster.DimensionY * 30 > totalHeight)
                totalHeight += raster.DimensionY * 30;

            let group = new Konva.Group({
                y: raster.Id * 30,
                x: xPosGroup,
                width: raster.DimensionX * 30,
                heigth: raster.DimensionY * 30,
                name: raster.Name,
                id: 'Group_' + raster.Id,
                visible: true,
                stroke: 'yellow',
                strokeWidth: 4

            });
            for(let j = 0; j < raster.Projections.length; j++) {
                let projection = raster.Projections[j];
                let xPosition:number = projection.RasterX * (projection.width * 3);
                let box = new Konva.Rect({
                    x: xPosition,
                    y: projection.RasterY * projection.height * 3,
                    id: 'Lamp_' + projection.LampId,
                    fill: projection.Color,
                    width: projection.width * 3,
                    height: projection.height * 3,
                    stroke: 'black',
                    strokeWidth: 2,
                });
                group.add(box);
            }
            Layer.add(group);
        }
        console.log(Layer);
        this.stage.add(Layer);
        this.stage.draw();
        if (totalWidth > this.configKonva.width || totalHeight > this.configKonva.height) {
            if (totalWidth > this.configKonva.width && totalHeight > this.configKonva.height)
            {
                this.xScale = this.configKonva.width / totalWidth;
                this.yScale = this.configKonva.height / totalHeight;
                this.scaleBy = (this.xScale * this.yScale) * 1000;
            }
            else if (totalWidth > this.configKonva.width) {
                this.xScale = this.configKonva.width / totalWidth;
                this.yScale = this.configKonva.width / totalWidth;
                this.scaleBy = (totalWidth / 10) / this.configKonva.width;
            }
            else {
                this.yScale = this.configKonva.height / totalHeight;
                this.xScale = this.configKonva.height / totalHeight;
                this.scaleBy = (totalHeight / 10) / this.configKonva.height;
            }
            let stageScale = { x: this.xScale, y: this.yScale };
            this.stage.scale(stageScale);
        }
        else {
            let stageScale = { x: 1, y: 1 };
            this.stage.scale(stageScale);
            this.scaleBy = 1.01;
        }
    }
    redrawRasters(rasters:any) {
        this.stage.clear();
        let layer = new Konva.Layer();
        let totalWidth = 0;
        let totalHeight = 0;
        let xPosGroup = rasters[0].Id;
        for (let i = 0; i < rasters.length; i++) {
            let raster = rasters[i];
            if (raster.DimensionX * 30 > totalWidth)
                totalWidth += raster.DimensionX * 30;
            if (raster.DimensionY * 30 > totalHeight)
                totalHeight += raster.DimensionY * 30;
        
            var group = new Konva.Group({
                y: raster.id * 30,
                x: xPosGroup,
                width: raster.dimensionX * 30,
                heigth: raster.dimensionY * 30,
                offset: { 'offsetX': raster.Id * 10, 'offsetY': raster.Id * 10 },
                name: raster.Name,
                id: 'Group_' + raster.id,
                visible: true
            });
            for (let j = 0; j < raster.projections.length; j++) {
                let projection = raster.Projections[j];
                let xPosition = projection.RasterX * (projection.width * 3);
            
                var box = new Konva.Rect({
                    x: xPosition,
                    y: projection.RasterY * projection.height * 3,
                    id: 'Lamp_' + projection.LampId,
                    fill: projection.Color,
                    width: projection.width * 3,
                    height: projection.height * 3,
                    stroke: 'white',
                    strokeWidth: 2,
                });
                group.add(box);
            }
            layer.add(group);
        }
        this.stage.add(layer);
        if (totalWidth > this.configKonva.width || totalHeight > this.configKonva.height) {
            if (totalWidth > this.configKonva.width && totalHeight > this.configKonva.height) {
                this.xScale = this.configKonva.width / totalWidth;
                this.yScale = this.configKonva.height / totalHeight;
                this.scaleBy = (this.xScale * this.yScale) * 1000;
            }
            else if (totalWidth > this.configKonva.width) {
                this.xScale = this.configKonva.width / totalWidth;
                this.yScale = this.configKonva.width / totalWidth;
                this.scaleBy = (totalWidth / 10) / this.configKonva.width;
            }
            else {
                this.yScale = this.configKonva.height / totalHeight;
                this.xScale = this.configKonva.height / totalHeight;
                this.scaleBy = (totalHeight / 10) / this.configKonva.height;
            }
            let stageScale = { x: this.xScale, y: this.yScale };
            this.stage.scale(stageScale);
        }
        else {
            let stageScale = { x: 1, y: 1 };
            this.stage.scale(stageScale);
            this.scaleBy = 1.01;
        }
    }
    playActiveSelector() {
        if (this.isPlaying)
            return 'icon-playScenario-inactive';
        else
            return 'icon-playScenario';
    }

    pauseActiveSelector() {
        if (this.playScenarioId == -1)
            return 'icon-pauseScenario-inactive';
        else {
            if (this.isPlaying) {
                return 'icon-pauseScenario';
            }
            else
                return 'icon-pauseScenario-inactive';
        }
    }
    stopActiveSelector() {
        if (this.playScenarioId == -1)
            return 'icon-stopScenario-inactive';
        else {
            if (this.isPlaying) {
                return 'icon-stopScenario';
            }
            else
                return 'icon-stopScenario-inactive';
        }
    }
    startStopAnimation() {
        let user = this.$store.state.auth.authentication.profile;
        if(user && user.role == 'user' || user.role=='admin') {
            let token = user.access_token;
            let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
            console.log(this.startStopAnim);
            this.startStopAnim = !this.startStopAnim;
            console.log(this.startStopAnim);

            let StartStopScheduler = {action: this.startStopAnim};
            instance.post('/Renderer/StartStopAnimation', StartStopScheduler, {headers});
        }
        else
            alert("Not enough rules for this action");
    }
    playScenario(scenario:any) {
        if (!scenario.isPlaying) {
            let user = this.$store.state.auth.authentication.profile;
            if(user && user.role == 'user' || user.role=='admin') {
                let token = user.access_token;
                this.isPlaying = true;
                this.playScenarioId = scenario.scenarioId;
                scenario.isPlaying = true;
                let ScenarioNameId = {
                    "scenarioId": scenario.scenarioId,
                    "scenarioName": scenario.scenarioName,
                    "elapsedTicks": this.sliderVal
                };
                let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
                instance.post('/Renderer/PlayScenario', ScenarioNameId, {headers});
            }
            else
                alert("Not enough rules for this action");
        }
    }
    pauseScenario(scenario:any) {
        if (scenario.isPlaying) {
            let user = this.$store.state.auth.authentication.profile;
            if(user && user.role == 'user' || user.role=='admin') {
                let token = user.access_token;
                console.log(scenario.scenarioId, scenario.scenarioName);
                scenario.isPlaying = false;
                this.isPlaying = false;
                let ScenarioNameId = {"scenarioId": scenario.scenarioId, "scenarioName": scenario.scenarioName};
                let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
                instance.post('/Renderer/PauseScenario', ScenarioNameId, {headers});
            }
            else
                alert("Not enough rules for this action");
        }
    }
    stopScenario(scenario:any) {
        if (scenario.isPlaying) {
            let user = this.$store.state.auth.authentication.profile;
            if(user && user.role == 'user' || user.role=='admin') {
                let token = user.access_token;
                console.log(scenario.scenarioId, scenario.scenarioName);
                scenario.isPlaying = false;
                this.isPlaying = false;
                let ScenarioNameId = {"scenarioId": scenario.scenarioId, "scenarioName": scenario.scenarioName};
                let headers = {"Accept": "application/json", "Authorization": "Bearer " + token};
                instance.post('/Renderer/StopScenario', ScenarioNameId, {headers});
                this.playScenarioId = -1;
                this.sliderVal = 0;
            }
            else
                alert("Not enough rules for this action");
        }
    }
    onItemSelected() {
        console.log(this.selected, this.groups);
        let findedRaster = this.groups.find(elem => elem.id == this.selected);
        console.log(findedRaster);
        this.selected = findedRaster;
        console.log(this.selected);
        let stageRef = this.stage.getStage();
        let layer = stageRef.children[0];
        let groupObjects = layer.children;
        if (this.selected.name !== 'All rasters') {
            for (let i = 0; i < groupObjects.length; i++) {
        
            let group = groupObjects[i];
            if (group.attrs.name != this.selected.name)
            group.visible(false);
            else
            group.visible(true);
        }
        }
        else {
            for (let i = 0; i < groupObjects.length; i++) {
                let group = groupObjects[i];
                group.visible(true);
            }
        }
    }

    onScenarioSelect() {
        console.log(this.selectedScenario);
        let findedScen = this.scenarios.find(f => f.ScenarioId == this.selectedScenario);
        this.selectedScenario = findedScen;
        console.log(findedScen);
        this.playScen = {
            scenarioId: findedScen.ScenarioId,
            scenarioName: findedScen.ScenarioName,
            isPlaying: findedScen.IsPlaying,
            scenarioTime: findedScen.ScenarioTime,
            totalTicks: findedScen.TotalTicks,
        };
        console.log(this.playScen);
        this.sliderMaxVal = this.playScen.totalTicks;
    }

    setDefScale() {
        /*let stageScale = { x: this.xScale, y: this.yScale };
        console.log(stageScale);
        this.stage.scale(stageScale);*/
        // do we need padding?
        let padding = 10;
        let layer = this.stage.children[0];
        // get bounding rectangle
        let box = layer.getClientRect({ relativeTo: this.stage });
    
        this.stage.setAttrs({
            x: box.x * this.xScale,
            y: box.y * this.yScale,
            scaleX: this.xScale,
            scaleY: this.yScale
        });
    }
}