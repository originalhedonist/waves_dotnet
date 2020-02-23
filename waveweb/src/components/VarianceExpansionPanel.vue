<template>
    <v-expansion-panels>
        <v-expansion-panel>
            <v-expansion-panel-header>
                {{title}}: progression {{variance.progression}}, randomness {{variance.randomness}}
            </v-expansion-panel-header>
            <v-expansion-panel-content>
                <VarianceEditor :variance="variance" />
            </v-expansion-panel-content>
        </v-expansion-panel>
    </v-expansion-panels>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop } from 'vue-property-decorator';
    import { Variance } from '../dtos';
    import VarianceEditor from '@/components/VarianceEditor.vue';
    import '@/dtos';
    @Component({
        components: {
            VarianceEditor,
        },
    })
    export default class VarianceExpansionPanel extends Vue {
        @Prop() public title: string;
        @Prop() public variance: Variance;
    }

</script>

<style scoped>
</style>
