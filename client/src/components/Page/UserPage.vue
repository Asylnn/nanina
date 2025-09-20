<script lang="ts">
import User from '@/classes/user/user';
import StatsPage from './StatsPage.vue';
import TreePage from './TreePage.vue';


export default {
    name : "UserPage",
    data(){
        return {
            page : 0
        }
    },
    props:{
        user : {
            type : User,
            required : true
        }
    },
    components:{
        StatsPage,
        TreePage,
    }
}


</script>
<template>
    <div id="userpage" class="margins flex">
        <div class="userHeader">
            <ul id="userPages">
                <div @click="page = 0" id="profile">
                    <div id="profileImg">
                        <img :src="`${user.avatarPATH}?size=64`">
                    </div>
                    
                    <span>{{ user.username }}</span>
                    <span>Lv. {{ user.lvl }}</span>
                </div>
                <li @click="page = 1"><span>Tree</span></li>
            </ul>
        </div>
        <div id="profileBody" >
            <div v-if="page == 0" >
                <StatsPage :user="user"></StatsPage>
                <img src="@/assets/ugly_coin.svg">
                <div class="amount">{{ user.money }}</div>
            </div>
            <div v-else-if="page == 1">
                <TreePage :user="user"></TreePage>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>

#profile
{
    display: grid;
    grid-template-columns: 1fr 4fr;
    align-items: center;
    text-align: left;
}

#profileImg
{
    grid-row-start: 1;
    grid-row-end: 3;
    width: 64px;
    height: 64px;
    border-radius: 64px;
    overflow: hidden;
}

/*#userpage
{
    margin-left: 20vw;
    margin-top: 5vh;
}*/

.userHeader{
    
    align-content: center;
}

#userPages {
    align-items: center;
    justify-content: center;
    display:grid;
    cursor: pointer;
    grid-template-columns: 1fr 1fr 1fr 1fr ;
}

#profileBody
{
    margin: 50px 5vw;
}



</style>