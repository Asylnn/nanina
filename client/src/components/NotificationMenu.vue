<script lang="ts">

import Notif from '@/classes/notif';

export default {
    name : "NotificationMenu",
    data() {
        return {
            showAll : true,
        }
    },
    props: {
        notifs : {
            type : Array<Notif>,
            required : true
        },
    },
    methods: {
        updateShowAll() {
            this.showAll = !this.showAll;
        }
    },
}


</script>

<template>
    <div id="notifMenu">
        <img src="../assets/fleche-vers-le-bas.png" alt="" @click="updateShowAll()">
        <div v-if="showAll">
            <div v-for="notif in notifs">
                <div class="notifElement">
                    <div id="notifType">{{ notif.type }}</div>
                    <div id="notifMessage">{{ notif.message }}</div>
                </div>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>

#notifMenu {
    border-radius: 35px;
    background-color: crimson;
    position: sticky;
    margin-left:5vw;
    width: 90vw;
    top: 15vh;
}
.notifElement {
    padding: 1vh 1vw;
    display: flex;
}
#notifType {
    width: 20%;
    padding-left: 5vw;
    float : left;
}
/*#clicker:hover {
    cursor: pointer;
}*/
img {
    position:sticky;
    width: 30px;
    height:30px;
    margin-left: 85vw;
}
</style>