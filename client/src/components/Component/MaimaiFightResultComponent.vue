<script lang="ts">
import ScoreDTO from '@/classes/maimai/scoreDTO';
import { MillisecondsToHourMinuteSecondFormat } from '@/classes/utils';



export default {
    name : "MaimaiFightResultComponent",
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
        <span>{{ score.song.name.en }} | {{ score.song.artist.en }} ({{  score.difficulty_level.label }})</span><br>
        <span>{{ score.score_formatted }}, {{ score.achievement_formatted }}% ({{ score.rank }})</span><br>
        <span>
            {{$t("activities.fight.result.timesave")}} : {{ MillisecondsToHourMinuteSecondFormat(score.timesave!*1000) }}
        </span>
    </div><br>
</template>

<style lang="css" scoped>

</style>