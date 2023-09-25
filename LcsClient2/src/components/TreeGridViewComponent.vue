<template>
    <div v-if="parentComponent == 'devicesComponent'">
        <table v-show="isOpen" v-if="isFolder" id="devicesTreeGrid">
            <thead>
                <tr>
                    <th></th>
                    <th class="td_label_tree_item">{{$t("message.deviceName")}}</th>
                    <th class="td_label_tree_item">{{$t("message.deviceLabel")}}</th>
                    <th class="td_label_tree_item">{{$t("message.deviceStatus")}}</th>
                    <th class="td_label_tree_item">{{$t("message.deviceDmxStartAddress")}}</th>
                    <th class="td_label_tree_item">{{$t("message.deviceFirmwareVersion")}}</th>
                    <th class="td_label_tree_item">{{$t("message.deviceFootprint")}}</th>
                    <th class="td_label_tree_item">{{$t("message.isInProject")}}</th>
                </tr>
            </thead>
            <tbody>
                <template v-if="item.Type == 'GatewayUniverse'">
                    <tr v-for="(child, index) in item.Children">
                        <td></td>
                        <td class="td_value_tree_item"><span @click="nodeClick(child)">{{ child.DisplayName }} : {{ child.Id}}</span></td>
                        <td class="td_value_tree_item">{{ child.Label }}</td>
                        <td class="td_value_tree_item"><span class="folder icon" :class="iconStatusSelector"></span></td>
                        <td class="td_value_tree_item">{{ child.DmxAddress }}</td>
                        <td class="td_value_tree_item">{{ child.SoftwareVersionIdLabel }}</td>
                        <td class="td_value_tree_item">{{ child.DmxFootprint }}</td>
                        <td class="td_value_tree_item"><input type="checkbox" disabled v-model="child.IsInProject" /></td>
                    </tr>
                </template>
                <tamplate v-else>
                    <tr>
                        <td v-if="isFolder" @click="toggle" class="td_label_tree_item">[{{ isOpen ? '-' : '+' }}]</td>
                        <td class="td_label_tree_item">{{ itemName }}</td>
                        <td class="td_label_tree_item"></td>
                        <td class="td_label_tree_item"></td>
                        <td class="td_label_tree_item"></td>
                        <td class="td_label_tree_item"></td>
                        <td class="td_label_tree_item"></td>
                        <td class="td_label_tree_item"></td>
                    </tr>
                    <tr v-for="child in item.Children">
                        <tree-grid :parentComponent="parentComp"
                                   :item="child"
                                   :nodeClick="selDeviceHandle"
                                   :writeToLog="onLogHandle"></tree-grid>
                    </tr>
                </tamplate>
            </tbody>
        </table>
    </div>
    <div v-else>

    </div>
</template>
<script>
    import $ from "jquery";
    import DataTable from "datatables.net";

    export default {
        name: 'TreeGridItem',
        components: {
            'dataTable': DataTable,
        },
        props: {
            item: Object,
            nodeClick: Function,
            parentComponent: String,
        },
        emits: ['selectDevice', 'writeToLog'],
        data: function () {
            return {
                isOpen: true,
                label: '',
                firstChildName: '',
                isActive: false,
                currentlyActiveItem: null
            };
        },
        mounted() {
            setTimeout(() => {
                console.log(this.parentComponent);
            }, 1000)
            $(document).ready(function () {
                $('#devicesTreeGrid').DataTable();
                $('.projDevices').DataTable();
            });

        },
        computed: {
            isFolder: function () {
                return this.item.Children && this.item.Children.length;
            },
            isFolder2: function () {
                return this.item.children && this.item.children.length;
            },
            isInProject: function () {
                return this.item.IsInProject;
            },
            isRdmDevice: function () {
                return this.item.Type == 'RdmDevice';
            },
            classSelector: function () {
                if (this.item.Children && this.item.Children.length && this.item.Type == 'GatewayUniverse')
                    return 'bold rdmDevice';
                else if (this.item.Children && this.item.Children.length && this.item.Type != 'GatewayUniverse')
                    return 'bold';
            },
            itemName: function () {
                if (this.item) {
                    switch (this.item.Type) {
                        case 'ArtNetGateway':
                            return this.item.LongName.trim() + ':' + this.item.Id;
                            break;
                        case 'ArtNetGatewayNode':
                            this.firstChildName = this.item.ShortName.trim('\0');
                            return this.item.ShortName.trim('\0') + ':' + this.item.Id;
                            break;
                        case 'GatewayUniverse':
                            let port = '';
                            if (this.item.PortType == 128)
                                port = 'Output';
                            else if (this.item.PortType == 64)
                                port = 'Input';
                            return 'DMX ' + port + ' ' + this.item.PortIndex + ' Address ' + this.item.Universe;
                            break;
                        case 'RdmDevice':
                            return this.item.DisplayName.trim() + ':' + this.item.Id;
                            break;
                        default:
                            return '';
                            break;
                    }
                }
            },
            iconStatusSelector: function () {
                switch (this.item.DeviceStatus) {
                    case 0:
                        return 'icon-lamp-normal';
                        break;
                    case 1:
                        return 'icon-lamp-new';
                        break;
                    case 2:
                        return 'icon-lamp-loading';
                        break;
                    case 3:
                        let level = 'warning';
                        let logInfo = { 'level': level, 'item': item };
                        this.$emit('writeToLog', logInfo);
                        return 'icon-lamp-warning';
                        break;
                    case 4:
                        let level2 = 'lostDevice';
                        let logInfoLost = { 'level': level2, 'item': item };
                        this.$emit('writeToLog', logInfoLost);
                        return 'icon-lamp-lost';
                        break;
                }
            }
        },
        watch: {

        },
        methods: {
            toggle: function () {
                if (this.isFolder) {
                    this.isOpen = !this.isOpen;
                }
            },
            toggle2: function () {
                if (this.isFolder2) {
                    this.isOpen = !this.isOpen;
                }
            },
            /*onClick(item) {
                this.currentlyActiveItem = item.Id;
            }*/
        }
    }
</script>