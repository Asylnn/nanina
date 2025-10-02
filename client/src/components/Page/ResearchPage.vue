<script lang="ts">
import User from '@/classes/user/user';
import ActivityType from '@/classes/user/activity_type';
import ResearchNode from '@/classes/research/research_nodes';
import ResearchNodeClient from '@/classes/research/research_nodes_client';
import ResearchNodeComponent from '../Component/ResearchNodeComponent.vue';
import Waifu from '@/classes/waifu/waifu';
import researchNodesJSON from  '@/../../save/research.json'

class Tree
{
    public elements !: ResearchNodeClient[]
    public width = 0

    constructor(rs : ResearchNodeClient[])
    {
        this.elements = rs
    }

    public getSize() : number
    {
        let tierCount = [0,0,0,0,0]
        this.elements.forEach(rn => {
            tierCount[rn.tier]++
        })
        return tierCount.reduce((a, b) => Math.max(a, b))
    }
}

class Path
{
    public startx !: number
    public starty !: number
    public endx   !: number
    public endy   !: number
}

export default {
    name : "ResearchPage",
    data(){
        return {
            researchNodes : Object.values(researchNodesJSON),
            ActivityType: ActivityType,
            publicPath : import.meta.env.BASE_URL,
            trees: [] as Tree[],
            paths: [] as Path[],
            researchNodesProcessed: [] as ResearchNodeClient[],
            selectedNode: null as ResearchNodeClient | null,
        }
    },
    props: {
        user: {
            type: User,
            required: true
        },
        selectedWaifu:{
            type:[Waifu, null],
            required:true,
        }
    },
    mounted()
    {   
        /*This code is absolutely disgusting*/
        
        /*Add some necessary properties inside the research nodes from the json by converting them to "ResearchNodeClient"*/
        for(let i in this.researchNodes)
        {
            this.researchNodes[i] = Object.assign(new ResearchNodeClient, this.researchNodes[i])
        }

        //Make a deep copy of researchNodes
        let researchNodeCopy = JSON.parse(JSON.stringify(this.researchNodes)) as Array<ResearchNodeClient>
        
        /*Create Trees based on nodes without any requirement (tree roots in some sense)*/ 
        
        for(var index = 0; researchNodeCopy.length > index; index++)
        {
            if(researchNodeCopy[index].requirements.length == 0)
            {
                this.trees.push(new Tree([researchNodeCopy[index]]))
                researchNodeCopy.splice(index, 1)
                index--
                console.log("generate tree")
            }
        }
        
        /*Add nodes into trees based on requirements (if the requirements are in the tree, then they are added in that tree)*/ 

        while(researchNodeCopy.length != 0)
        {
            this.trees.forEach(tree =>
            {
                researchNodeCopy.forEach((researchNode, index) => 
                {
                    if(researchNode.requirements.some(id => tree.elements.some(element => id == element.id)))
                    {

                        /* This portion of code is for filling the "allRequirements" and "requiredBy" fields */
                        researchNode.requirements.forEach(id =>  {
                            var previousRN = tree.elements.find(elem => elem.id == id)!
                            previousRN.requiredBy.push(researchNode.id)
                            researchNode.allRequirements.push(...previousRN.allRequirements)
                        })
                        researchNode.allRequirements.push(...researchNode.requirements)
                        

                        console.log(researchNode)
                        tree.elements.push(researchNode)
                        researchNodeCopy.splice(index, 1)
                        console.log("generate node")
                    }
                })
            })
        }

        /*Compute the nodes's positions (it's devious)*/ 

        function computeNodePositionRecursive(tree : Tree, rn : ResearchNodeClient, size : number) : number /* Size of the subtree from that node */
        {

            rn.leftPos = (rn.tier - 1)*600 + 200
            rn.topPos =  size*135
            
            rn.requiredBy.forEach(reqID => {
                let subTreeNodes = tree.elements.filter(subRN => subRN.allRequirements.some(id => id == reqID) || subRN.id == reqID)
                let subTree = new Tree(subTreeNodes)
                console.log(subTree, reqID)
                let subNode = subTree.elements.find(subRN => subRN.id == reqID)!
                size = computeNodePositionRecursive(subTree, subNode, size)
            })

            return size + +(rn.requiredBy.length == 0)
        }

        let size = 1
        this.trees.forEach(tree => {
            var rootNode = tree.elements.find(rn => rn.requirements.length == 0)!
            size = computeNodePositionRecursive(tree, rootNode, size)
            size += 1
        })


        /*Put all the nodes inside all the trees and put them into an array (like in the prop)*/ 

        this.researchNodesProcessed = this.trees.reduce((rns, tree) => [...rns, ...tree.elements] , [] as ResearchNodeClient[])


        /*Compute the paths between nodes*/ 

        this.researchNodesProcessed.forEach(node => {
            node.requirements.forEach(requirement => {
                let requiredNode = this.researchNodesProcessed.find(rs => rs.id == requirement)!
                let path = new Path
                path.startx = requiredNode.leftPos + 350
                path.starty = requiredNode.topPos + 40
                path.endx = node.leftPos
                path.endy = node.topPos + 40
                this.paths.push(path)
            })
        })
        
    },
    methods:{
        GetPosition(researchNode: ResearchNodeClient)
        {
            let style = ""

            style += `left:${researchNode.leftPos}px;`
            style += `top:${researchNode.topPos}px;`
            
            return style;
        },
        GetSVGWidth()
        {
            return this.paths.reduce((leftPos, path) => Math.max(leftPos, path.endx), 0)
        },
        GetSVGHeight()
        {
            return this.paths.reduce((leftPos, path) => Math.max(leftPos, path.endy), 0)
        },
        showWaifuSelector(rn : ResearchNodeClient)
        {
            let isResearchNotLocked = rn.requirements.every(researchID => this.user.isResearchDone(researchID))
            let isResearchNotAlreadyResearchedOrInfinite = ! this.user.isResearchDone(rn.id) || rn.infinite
            let isResearchNotInProgress = this.user.activities.every(activity => activity.researchID != rn.id)
            if(isResearchNotLocked && isResearchNotAlreadyResearchedOrInfinite && isResearchNotInProgress)
            {
                this.$emit("show-waifu-selector")
                this.selectedNode = rn
            }
        }
    },
    components:{
        ResearchNodeComponent
    },
    emits: ["show-waifu-selector", "reset-selected-waifu"],
}


</script>
<template>
    <div id="researchPage" v-dragscroll>
        <svg :width="GetSVGWidth()" :height="GetSVGHeight()">
            <polyline v-for="p in paths" :points="`${p.startx},${p.starty} ${p.endx},${p.endy}`"></polyline>    
        </svg>
        <div v-for="researchNode in researchNodesProcessed">
            <ResearchNodeComponent 
                v-on:show-waifu-selector="showWaifuSelector(researchNode)" 
                v-on:reset-selected-waifu="$emit('reset-selected-waifu')"
                data-no-dragscroll class="rn" 
                :style="GetPosition(researchNode)" 
                :research-node="researchNode" 
                :user="user"
                :selected-waifu="researchNode.id == selectedNode?.id ? selectedWaifu : null">
            </ResearchNodeComponent>
        </div>
    </div>
</template>

<style lang="css" scoped>

#researchPage
{
    cursor:all-scroll;
    position: absolute;
    height:85%;
    width:92%;
    left:4%;
    overflow: hidden
}

.rn
{
    position:absolute;
}

polyline {
    fill: none;
    stroke-width: 10;
    stroke-linecap: round;
    stroke: white;
}

</style>