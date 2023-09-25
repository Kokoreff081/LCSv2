<template>
    <!-- Navbar -->
    <nav class="main-header navbar navbar-expand navbar-white navbar-light">
        <!-- Left navbar links -->
        <ul class="navbar-nav">
            <li class="nav-item">
                <a class="nav-link" data-widget="pushmenu" href="#" role="button">
                    <i class="fas fa-bars"></i>

                </a>
            </li>
            <li class="nav-item d-none d-sm-inline-block">
                <a href="#" class="nav-link">{{ $t("message.welcome") }}</a>
            </li>
            <!--<li class="nav-item d-none d-sm-inline-block">
        <a href="#" class="nav-link">Contact</a>
    </li>-->
            <li class="nav-item">
                <select class="form-control" v-model="selectedLang" @change="$emit('selectLang', selectedLang)">
                    <option v-for="lang in langs" :value="lang.value">
                        {{lang.text}}
                    </option>
                </select>
            </li>

            <li class="nav-item">
                <clock></clock>
            </li>
            <li class="nav-item">
                <span class="task">{{$t("message.nowPlaying")}}</span>
                <span class="task">{{playingNow}}</span>
            </li>
            <li class="nav-item">
                <span class="task">{{$t("message.nextPlaying")}}</span>
                <span class="task">{{nextToPlay}}</span>
            </li>
        </ul>


        <!-- Right navbar links -->
        <ul class="navbar-nav ml-auto">
            <!-- Navbar Search -->
            <li class="nav-item">
                <a class="nav-link" data-widget="navbar-search" href="#" role="button">
                    <i class="fas fa-search"></i>
                </a>
                <div class="navbar-search-block">
                    <form class="form-inline">
                        <div class="input-group input-group-sm">
                            <input class="form-control form-control-navbar" type="search" placeholder="Search" aria-label="Search">
                            <div class="input-group-append">
                                <button class="btn btn-navbar" type="submit">
                                    <i class="fas fa-search"></i>
                                </button>
                                <button class="btn btn-navbar" type="button" data-widget="navbar-search">
                                    <i class="fas fa-times"></i>
                                </button>
                            </div>
                        </div>
                    </form>
                </div>
            </li>

            
            <!-- Notifications Dropdown Menu -->
            <li class="nav-item dropdown">
                <a class="nav-link" data-toggle="dropdown" href="#">
                    <i class="far fa-bell"></i>
                    <span class="badge navbar-badge" id="notifier">15</span>
                </a>
                <div class="dropdown-menu dropdown-menu-lg dropdown-menu-right">
                    <span class="dropdown-item dropdown-header">15 Notifications</span>
                    <div class="dropdown-divider"></div>
                    <a href="#" class="dropdown-item">
                        <i class="fas fa-envelope mr-2"></i> 4 new messages
                        <span class="float-right text-muted text-sm">3 mins</span>
                    </a>
                    <div class="dropdown-divider"></div>
                    <a href="#" class="dropdown-item">
                        <i class="fas fa-users mr-2"></i> 8 friend requests
                        <span class="float-right text-muted text-sm">12 hours</span>
                    </a>
                    <div class="dropdown-divider"></div>
                    <a href="#" class="dropdown-item">
                        <i class="fas fa-file mr-2"></i> 3 new reports
                        <span class="float-right text-muted text-sm">2 days</span>
                    </a>
                    <div class="dropdown-divider"></div>
                    <a href="#" class="dropdown-item dropdown-footer">See All Notifications</a>
                </div>
            </li>
            <li class="nav-item">
                <a class="nav-link" data-widget="fullscreen" href="#" role="button">
                    <i class="fas fa-expand-arrows-alt"></i>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" data-widget="control-sidebar" data-slide="true" href="#" role="button">
                    <i class="fas fa-th-large"></i>
                </a>
            </li>
            
        </ul>
    </nav>
    <!-- /.navbar -->
</template>

<script>
    import Dropdown from 'primevue/dropdown';
    import Clock from '../components/common/Clock.vue';
    import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
export default {
    name: "Navbar",
        props: ["playingNow", "nextToPlay"],
        components: {
            'myselect': Dropdown,
            'clock': Clock,
        },
        data() {
            return {
                langs: [{ value: 'ru', text: 'русский' }, { value: 'en', text: 'english' }],
                selectedLang: this.$i18n.locale,
                connFlag: false,
                connection: null,
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
        mounted() {
            this.connection.on('NewLogEntries', (entriesCount) => {
                console.log(entriesCount);
                let that = this;
                if (entriesCount.level == 'Error') {
                    $('#notifier').removeClass('badge-warning');
                    $('#notifier').addClass('badge-danger');
                }
                else if (entriesCount.level == 'Warning') {
                    $('#notifier').removeClass('badge-danger');
                    $('#notifier').addClass('badge-warning');
                }
                else {
                    $('#notifier').removeClass('badge-danger');
                    $('#notifier').addClass('badge-primary');
                }
                $('#notifier').text(entriesCount.count);
            });
        }

}
</script>

<style>
</style>