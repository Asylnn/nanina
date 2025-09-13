<script lang="ts">
import User from '@/classes/user/user';
import StatsPage from './StatsPage.vue';
import TreePage from './TreePage.vue';


export default {
    name : "ActivitiesHelpComponent",
    data(){
        return {
        }
    },
    components:{
    }
}


</script>
<template>
    <div class="margins">
        <h3>{{$t("activities.help.title")}}</h3><br>
        <p>{{$t("activities.help.overview")}}</p><br>
        <p>{{$t("activities.help.activities_limit")}}</p><br>
        <p>{{$t("activities.help.companion_can_only_do_one_action")}}</p><br>
        <p>{{$t("activities.help.activities")}}</p><br>
        <p>{{$t("activities.help.all_the_different_activities")}}</p><br>
        <ul>
            <li><p>{{$t("activities.help.cafe_explanation")}}</p></li><br>
            <li><p>{{$t("activities.help.mining_explanation")}}</p></li><br>
            <li><p>{{$t("activities.help.exploration_explanation")}}</p></li><br>
            <li><p>{{$t("activities.help.crafting_explanation")}}</p></li><br>
            <li><p>{{$t("activities.help.research_explanation")}}</p></li><br>
        </ul>
        <p>{{$t("activities.help.fighting")}}</p><br>
        <p>{{$t("activities.help.fighting2")}}</p><br>
        <p>{{$t("activities.help.fighting_osu")}}</p><br><br><br><br>
    </div>
</template>

<style lang="css" scoped>

.margins
{
    margin-top: 20px;
}

h3
{
    text-align: center;
}

</style>