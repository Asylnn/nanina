const vm = Vue.createApp({
    data() {
        return {
            id: 1,
            imgURL: "https://cdn.discordapp.com/attachments/383035330082897930/1290404910399098982/GYrXGACboAACxp7.jpg?ex=66fc56b9&is=66fb0539&hm=4c024677ac1184c917b60b02be9873f099a45795d8d350219aa13f2c5068b6e5&", 
            objectType: "Waifu",
            xp: 0,
            lvl: 1,
            b_int: 69,
            b_luck: 17,
            b_exp: 52,
            stars: 4,
            modificators: [],
            equipedItems: [],
            action : {},
            owner : "Le seul l'unique",
            name: "Thighs-sama",
            diffLvlup: 10,
            o_exp: 2.1,
            o_int: 2.2,
            o_luck: 2.3,
            u_exp: 3.1,
            u_int: 3.2,
            rarity: 4,
            value: -1,
            isTradable: false,


            theme: "dark_theme"
        }
    },
    methods: {
    },
    computed: {
    },
    watch: {
    }
}).mount('#app')