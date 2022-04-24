package com.Delivery_Project.ui.ui.fragment

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.LinearLayout
import android.widget.TextView
import androidx.constraintlayout.widget.ConstraintLayout
import androidx.fragment.app.Fragment
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.Delivery_Project.R
import com.Delivery_Project.adapter.OrderAdapter
import com.Delivery_Project.database.OrderHelper
import com.Delivery_Project.databinding.FragmentCartBinding
import com.Delivery_Project.databinding.FragmentMenuBinding


class CartFragment : Fragment() {
    private var adapter : OrderAdapter? = null
    private lateinit var orderHelper: OrderHelper
    private lateinit var recyclerView: RecyclerView

    override fun onCreateView(
            inflater: LayoutInflater,
            container: ViewGroup?,
            savedInstanceState: Bundle?
    ): View? {
        val root: View = inflater.inflate(R.layout.fragment_cart, container, false)

        return root
    }


    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        orderHelper = OrderHelper(requireContext())
        var orderList = orderHelper.getAllOrders()
        recyclerView = requireView().findViewById(R.id.recyclerviewOrders)
        //recyclerView.layoutManager = LinearLayoutManager(activity)
        adapter = OrderAdapter()
        recyclerView.adapter = adapter
        adapter?.addItems(orderList)
        var totalCost: TextView = requireView().findViewById(R.id.total_cost)
        totalCost.text = orderHelper.getTotalCost().toString()
    }
}