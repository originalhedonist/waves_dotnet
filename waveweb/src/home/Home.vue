<template>
    
    <v-container>
        <v-expansion-panels >
            <v-expansion-panel>
                <v-expansion-panel-header>
                    Track details
                </v-expansion-panel-header>
                <v-expansion-panel-content>
                    <v-container fluid>
                        <v-row>
                            <v-slider v-model="Request.trackLengthMinutes"
                                      label="Track length (minutes)"
                                      thumb-label="always"
                                      min="1"
                                      max="60" />

                        </v-row>
                    </v-container>

                </v-expansion-panel-content>
            </v-expansion-panel>
            <v-expansion-panel>
                <v-expansion-panel-header>
                    Section details
                </v-expansion-panel-header>
                <v-expansion-panel-content>
                    <VarianceEditor/>
                </v-expansion-panel-content>
            </v-expansion-panel>
        </v-expansion-panels>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Prop, Watch } from 'vue-property-decorator';
    import { client } from '../shared';
    import { Hello, CreateFileRequest } from '../dtos';
    import '@/dtos';
    import VarianceEditor from '../components/VarianceEditor.vue';
    @Component({
        components: {
            VarianceEditor,
        },
    })
    export default class HomeComponent extends Vue {
        @Prop() public name: string;
        public txtName: string = this.name;
        public result: string = '';
        public Request: CreateFileRequest = new CreateFileRequest({
            trackLengthMinutes: 20,
        });

        public activated() {
            this.nameChanged(this.name);
        }

        @Watch('txtName')
        public onNameChanged(value: string, oldValue: string) {
            this.nameChanged(value);
        }

        public async nameChanged(name: string) {
            if (name) {
                const request = new Hello();
                request.name = name;
                request.theNumber = 45;
                const r = await client.post(request);
                this.result = r.result;
            } else {
                this.result = '';
            }
        }
    }
</script>

<style lang="scss">
    @import '../app.scss';

    .result {
        margin: 10px;
        color: darken($navbar-background, 10%);
    }
</style>