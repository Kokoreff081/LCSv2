<script setup>
import { HTTP } from '../global/commonHttpRequest';
import Devices from '../components/devices/Devices.vue';
/*import Project from './components/Project/Project.vue';
import Scheduler from './components/Project/Scheduler.vue';
import Renderer from './components/Project/Renderer.vue';*/
import Main from '../components/MainComponent.vue';
// import Graphic from './components/GraphicsComponent/GraphicComponent.vue';
import Clock from '../components/common/Clock.vue';
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

import Dropdown from 'primevue/dropdown';

import Preloader from "../dashboard/Preloader.vue"
import ContentHeader from "../dashboard/ContentHeader.vue"
import Navbar from "../dashboard/Navbar.vue";
import Footer from "../dashboard/Footer.vue";
import ControlSidebar from "../dashboard/ControlSidebar.vue";
</script>

<template>
  <main>
      <div class="wrapper">

          <Preloader></Preloader>
          <Navbar @selectLang="selectLang" :playingNow="playingNow" :nextToPlay="nextToPlay"></Navbar>
          <!--        <Sidebar :baseUrl="appBaseUrl" :rdmEnabled="configValues.isRdmEnabled" @showComponent="showComponent"></Sidebar>-->
          <aside class="main-sidebar sidebar-dark-primary elevation-4">
              <!-- Brand Logo -->
              <a href="#" class="brand-link">

                  <span class="brand-text font-weight-light">Light Control Service v. 2</span>
              </a>

              <!-- Sidebar -->
              <div class="sidebar">
                  <!-- Sidebar user panel (optional) -->
                  <div class="user-panel mt-3 pb-3 mb-3 d-flex">
                      <div class="image">

                      </div>
                      <div class="info">
                          <a href="#" class="d-block">{{ user }}</a>
                      </div>
                  </div>

                  <!-- SidebarSearch Form -->
                  <div class="form-inline">
                      <div class="input-group" data-widget="sidebar-search">
                          <input class="form-control form-control-sidebar" type="search" placeholder="Search" aria-label="Search">
                          <div class="input-group-append">
                              <button class="btn btn-sidebar">
                                  <i class="fas fa-search fa-fw"></i>
                              </button>
                          </div>
                      </div>
                  </div>

                  <!-- Sidebar Menu -->
                  <nav class="mt-2">
                      <ul class="nav nav-pills nav-sidebar flex-column" data-widget="treeview" role="menu" data-accordion="false">
                          <li class="nav-item menu-open">
                              <router-link to="/" class="nav-link active">
                                  <i class="nav-icon fas fa-tachometer-alt"></i>
                                  <p>
                                      {{$t('message.dashboard')}}
                                      <i class="right fas fa-angle-left"></i>
                                  </p>
                              </router-link>
                              <!--<router-link to="/">Dashboard</router-link>-->
                          </li>
                          <li class="nav-item" v-if="!deviceEnabled">
                              <a href="#" class="nav-link">
                                  <i class="nav-icon fas fa-th"></i>
                                  <p>
                                      {{$t('message.devices')}}
                                      <i class="right fas fa-angle-left"></i>
                                      <!--<span class="right badge badge-danger">New</span>-->
                                  </p>
                              </a>
                              <ul class="nav nav-treeview">
                                  <li class="nav-item">
                                      <router-link to="/deviceslist" class="nav-link" >
                                          <i class="far fa-circle nav-icon"></i>
                                          <p>Devices v1</p>
                                      </router-link>
                                      <!-- <router-link to="/deviceslist">{{$t('message.devices')}}</router-link>-->
                                  </li>
                                  <li class="nav-item">
                                      <a href="#" class="nav-link">
                                          <i class="far fa-circle nav-icon"></i>
                                          <p>Devices v2</p>
                                      </a>
                                  </li>
                              </ul>
                          </li>
                          <li class="nav-item">
                              <a href="#" class="nav-link">
                                  <i class="nav-icon fas fa-copy"></i>
                                  <p>
                                      {{$t('message.project')}}
                                      <i class="fas fa-angle-left right"></i>
                                  </p>
                              </a>
                              <ul class="nav nav-treeview">
                                  <li class="nav-item">
                                      <router-link to="/project" class="nav-link">
                                          <i class="far fa-circle nav-icon"></i>
                                          <p>{{$t('message.project')}}</p>
                                      </router-link>
                                      <!--<router-link to="/project">{{$t('message.project')}}</router-link>-->
                                  </li>
                                  <li class="nav-item">
                                      <router-link to="/scheduler" class="nav-link" @click="$emit('showComponent', 'scheduler')">
                                          <i class="far fa-circle nav-icon"></i>
                                          <p>{{$t('message.projectProjectScheduler')}}</p>
                                      </router-link>
                                  </li>
                                  <li class="nav-item">
                                      <router-link to="/renderer" class="nav-link" >
                                          <i class="far fa-circle nav-icon"></i>
                                          <p>{{$t('message.projectProjectScenPlayer')}}</p>
                                      </router-link>
                                  </li>
                              </ul>
                          </li>
                      </ul>
                  </nav>
                  <!-- /.sidebar-menu -->
              </div>
              <!-- /.sidebar -->
          </aside>
          <div class="content-wrapper" style="height:85%">
              <section class="content">
                  <div class="container-fluid">
                      <router-view></router-view>
                  </div>
              </section>
          </div>
          <Footer></Footer>
          <ControlSidebar></ControlSidebar>
      </div>
  </main>
</template>
