   M S B A M L   `   `   `  ����F  BTestTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:TestTracker.Controls.GridTestTracker.Controls.Grid  5      6    0TestTracker.Controls.NewWindows.TestResultWindow   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      8grid'clr-namespace:TestTracker.Controls.Grid     5      .��5	      �    WindowBackgroundBrush#���   6     � ColumnDefinitions  5
   
   �� 5      $	��540 �6   �� 5       � RowDefinitions 5   
   �� 5      .+�5   
   �� $
��Left=�6   $��0��6,   .+�5      �� 
 �� Top$ 5��6    �� Left$ 20��6(   .+�5      �� $��TEST RESULT ��6   $��4,4,4,4q�60   $��26(�6A   ]� 5      .+�5      �� $��Script name: ��6   $
��Left=�64   $��	10,25,0,5q�6O   $	��Top=�6b   �� 5      -   5         ��Name$ _scriptNameLabel��5      $��Script name��66   $
��Left=�6L   $��	10,25,0,5q�6g   $	��Top=�6z   
��� ���6�   5      ]� 5      .+�5      �� $��Total Tests: ��6   $
��Left=�64   $��10,5,0,5q�6P   $	��Top=�6b   �� 5      -   5        $ _totalTestResultLabel��5      $��Device name��6;   $
��Left=�6Q   $��10,5,0,5q�6m   $	��Top=�6   
��� ���6�   5      ]� 5      .+�5      �� $��Device name: ��6   $
��Left=�64   $��10,5,0,5q�6P   $	��Top=�6b   �� 5      -   5        $ _deviceNameLabel��5      $��Device name��66   $
��Left=�6L   $��10,5,0,5q�6h   $	��Top=�6z   
������6�   5      ]� 5       .+�5!      �� $��Serial number: ��6   $
��Left=�66   $��10,5,0,5q�6R   $	��Top=�6d   �� 5"      -   5        $ _serialNumberLabel��5"      $��Serial number��68   $
��Left=�6P   $��10,5,0,5q�6l   $	��Top=�6~   
������6�   5#      ]� 5$      .+�5%      �� $��FW Revision: ��6   $
��Left=�64   $��10,5,0,5q�6P   $	��Top=�6b   �� 5&      -   5        $ _FWRevisionLabel��5&      $��FW Revision��66   $
��Left=�6L   $��10,5,0,5q�6h   $	��Top=�6z   
������6�   5'      ]� 5(      .+�5)      �� $��
HBA name: ��6   $
��Left=�61   $��10,5,0,5q�6M   $	��Top=�6_   �� 5*      -   5        $ _hBANameLabel��5*      $��HBA name��63   $
��Left=�6F   $��10,5,0,5q�6b   $	��Top=�6t   
������6�   5+      ]� 5,      .+�5-      �� $��Total LBA: ��6   $
��Left=�62   $��10,5,0,5q�6N   $	��Top=�6`   �� 5.      -   5        $ _totalLBALabel��5.      $��	Total LBA��64   $
��Left=�6H   $��10,5,0,5q�6d   $	��Top=�6v   
������6�   5/      ]� 50      .+�51      �� $��
Capacity: ��6   $
��Left=�61   $��10,5,0,5q�6M   $	��Top=�6_   �� 52      -   5        $ _capacityLabel��52      $��Capacity��64   $
��Left=�6G   $��10,5,0,5q�6c   $	��Top=�6u   
������6�   53      ]� 54      $��10, 10, 5, 10q�6   55      56      �� 57      $	 290��6   $ 29��6*   .+�58      6   0TestTracker.Controls.Grid.DocumentViewerDataGrid  -	   5          Name$ _documentViewerDataGrid��58   .   $	��495��6O   $	��449��6[   59      5:      �� 5;   
   $��right=�6   $��0��6-   .+�5<      �� $ 70��6   .+�5=      6   0TestTracker.Controls.Grid.TestUnitResultDataGrid  -
   5          Name$ _testUnitResultDataGrid��5=   .   5>      5?      5@      5A      