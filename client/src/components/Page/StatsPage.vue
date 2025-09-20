<script lang="ts">

import User from '@/classes/user/user';

export default {
    name:"UserOptionPage",
    data() {
        return {
        }
    },
    props:{
        user : {
            type : User,
            required : true,
        }
    },
}

</script>



<template>
    <h2>Stats</h2><br>
    <div class="stat">
        <span>{{ $t("stats.nb_std_claim") }}</span> <span> {{ user.statCount.std_claim_count}} </span>  <br>
    </div>
    <div class="stat">
        <span>{{ $t("stats.nb_maimai_claim") }}</span> <span> {{ user.statCount.maimai_claim_count }} </span>  <br>
    </div>
    <div class="stat">
        <span>{{ $t("stats.continuous_fight_claim") }}</span> <span> {{ user.statCount.continuous_fight_count }} </span>  <br>
    </div>
    <div class="stat">
        <span>{{ $t("stats.nb_claim") }}</span> <span> {{ user.totalClaims }} </span>  <br><br>
    </div>
    <div class="stat">
        <span>{{ $t("stats.nb_dungeon_clear") }}</span> <span> {{ user.statCount.total_cleared_dungeon }} </span>  <br>
    </div>
    
</template>

<style lang="css" scoped>
.stat {
    display: grid;
    grid-template-columns: 8fr 1fr;
}
</style>