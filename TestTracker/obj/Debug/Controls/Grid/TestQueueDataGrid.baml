   M S B A M L   `   `   `  ����F  BTestTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:TestTracker.DataConverterTestTracker.DataConverter  5      I)clr-namespace:TestTracker.Controls.EditorTestTracker.Controls.Editor  5      E'clr-namespace:TestTracker.Controls.GridTestTracker.Controls.Grid  5	      1    +TestTracker.Controls.Grid.TestQueueDataGrid   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      9local'clr-namespace:TestTracker.DataConverter     5      EtestQueueEditor)clr-namespace:TestTracker.Controls.Editor     5      5l'clr-namespace:TestTracker.Controls.Grid     5	      $	��773��6@   $	��450��6L     � 	Resources  5      3   -TestTracker.DataConverter.TestStatusConverter   ToTestStatusString8   2TestTracker.DataConverter.ShortScriptNameConverter  ToShortScriptNameConverter7   1TestTracker.DataConverter.ForeBackgroundConverter  ToForeBackgroundConverter=   7TestTracker.DataConverter.TestStatusVisibilityConverter # ToTestStatusVisibilityConverter��%t   &	        &	      &	 &     &	 9       5   
   6$     5   
   6)     5   
   6(     5   
   6.   5      .��5      � .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ _testQueueDataGrid��5      ���	#FF393939$��0q�6F   
����� �   HeadersVisibility$ Column=�5      
�������   AutoGenerateColumns .     
IsReadOnly . ��5      �� *
TestQueues+   Columns 5      4  .System.Windows.Controls.DataGridTemplateColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Script Name��6)      CellTemplate 5      �� 5      .R�5      �� $��Center=�6�   ��6$   �� *
ScriptName+ �� 	Converter# [ 	 � ToolTip	 6z   �� *
ScriptName+6�   5      5        5      $ Status��6)    5      �� 5      .R�5       �� $��Center=�5!   ~   ��5    $   �� *TestStatusId+# [  ��5!   $   �� *TestStatusId+# [ 6~   5"      5$      0  *System.Windows.Controls.DataGridTextColumn  5%      $ Started Time��5&   :   1	  +System.Windows.Controls.DataGridBoundColumn
 	  Binding
 6   ��  �� Path$ StartedTime�6:     5'      $ Finished Time��5(   ;   
 6   �� $ FinishedTime�6;     5)      $ Edit HBA Configuration��6)    5*      �� 5+      .R�5,      �� $
��Edit��6N   ��6%   �� *TestQueueId+6N   5-      5/        50      $ View Test Result��6)    51      �� 52      .R�53      �� -   5        $
��View��53   �   ��68   �� *TestQueueId+y�6a   �� *TestStatusId+# [ 6�   54      56         RowDetailsTemplate 58      �� 59      .R�5:      1
   +TestTracker.Controls.Editor.TestQueueEditor
   
 Name$ _testQueueDataGrid��66   1   +TestTracker.Controls.Editor.TestQueueEditor   Feedback Feedback_Received6v    
  TestQueueId 6R   �� *TestQueueId+6�   5;      5=      5>      5?      