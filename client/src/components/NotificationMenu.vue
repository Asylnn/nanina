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
        },
        deleteMenu() {
            this.notifs.length = 0
        }
    },
}


</script>

<template>
    <div id="notifMenu">
        <div v-if="notifs[0] != null">
            <div id="firstLine">
                <div @click="updateShowAll()"></div>
                <img id="arrow" src="../assets/fleche-vers-le-bas.png" alt="" @click="updateShowAll()">
                <img id="cross" src="../assets/x.png" alt="" @click="deleteMenu()">
            </div>
                
            <div v-if="showAll">
                <div v-for="notif in notifs">
                    <div class="notifElement">
                        <div id="notifType">{{ notif.type }}</div>
                        <div id="notifMessage">{{ notif.message }}</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>

#notifMenu {
    border-radius: 35px;
    background-color: crimson;
    position: fixed;
    left: 5vw;
    top: 7.5vh;
    width: 90vw;
    z-index: 9999;
}
#firstLine {
    display: grid;
    grid-template-columns: 25fr 1fr 1fr;
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
    align-self: center;
}
#arrow {
    padding: 1vh 0;
    width: 30px;
}
#cross {
    width: 23px;
}
</style>