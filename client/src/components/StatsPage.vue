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
    <div class="stat">
        <span>Number of STD claims</span> <span> {{ user.statCount.std_claim_count}} </span>  <br>
    </div>
    <div class="stat">
        <span>Number of Maimai claims</span> <span> {{ user.statCount.maimai_claim_count }} </span>  <br>
    </div>
    <div class="stat">
        <span>Total claims</span> <span> {{ user.totalClaims }} </span>  <br><br>
    </div>
    <div class="stat">
        <span>Total number of dungeons cleared</span> <span> {{ user.statCount.total_cleared_dungeon }} </span>  <br>
    </div>
    
</template>

<style lang="css" scoped>
.stat {
    display: grid;
    grid-template-columns: 1.5fr 1fr;
}
</style>