<template>
    <div>
        
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { Breaks } from '../dtos';

    import '@/dtos';
    @Component
    export default class BreaksEditor extends Vue {
        @Prop() public breaks: Breaks;
    }



</script>

<style scoped>
    
</style>
