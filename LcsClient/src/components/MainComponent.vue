<template>
    <div class="row">
        <div class="col-sm-1">{{this.projectName}}</div>
        <div class="col-sm-11">
<!--            <ProjectChanger @refreshProjectEvent="$emit('refreshingProjectEvent')" :projectsProps="projects" :current="currentProject"></ProjectChanger>-->
        </div>
    </div>
  <!--<div class="row">
      
  </div>-->
    <div class="row">
        <Log></Log>
    </div>
</template>
<script>
import Log from './LogComponent/Log.vue';
//import ProjectChanger from './Project/ProjectChanger.vue';
export default {
    name: "Main",
    props: ["projectName", "projectInfo"],
    components: {
        Log,
        //ProjectChanger,
    },
    data() {
        return {
            projectName: this.projectName,
            info: this.projectInfo,
            projects: null,
        }
    }
}
</script>
<style>

</style>