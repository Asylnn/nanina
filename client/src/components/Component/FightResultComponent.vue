<script lang="ts">
import ScoreDTO from '@/classes/osu/scoreDTO';
import { MillisecondsToHourMinuteSecondFormat } from '@/classes/utils';



export default {
    name : "FightResultComponent",
    data(){
        return {
            MillisecondsToHourMinuteSecondFormat : MillisecondsToHourMinuteSecondFormat,
        }
    },
    mounted() {
    },
    props: {
        score : {
            type : ScoreDTO,
            required: true
        },
    },
    methods :{
    }
}


</script>
<template>
    <div>
        <span>{{ score.title }} | {{ score.artist }} [{{ score.creator }}] {{ score.version }}</span><br>
        <span>{{ score.rank }}   {{ Math.floor(score.accuracy*10000)/100 }}%</span>
        <div v-for="mod in score.mods"> Mods :
            <span>{{ mod }}, </span>
        </div>
        <span>{{ score.max_combo }}/{{ score.map_max_combo }}   {{ score.count_miss }}X</span><br>
        <span>{{ score.hit_length }}s | {{ score.difficulty_rating }}★ </span><br>
        <span>
            {{$t("activities.fight.result.timesave")}} : {{ MillisecondsToHourMinuteSecondFormat(score.timesave!*1000) }}s
        </span>
    </div>
</template>

<style lang="css" scoped>

</style>